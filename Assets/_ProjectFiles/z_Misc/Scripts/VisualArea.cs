using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class VisualArea : MonoBehaviour
{
    [SerializeField]
    private bool visualize = false;

    [SerializeField]
    private Material overrideMaterial;
    
    [SerializeField] private BoxCollider boxCollider;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Vector3[] originalVertices;
    
    public BoxCollider Collider => boxCollider;

    public bool Visualuze
    {
        get => visualize;
        set
        {
            visualize = value;
            UpdateVisibility();
        }
    }
    
    private void Awake()
    {
        Visualuze = false;
        Initialize();
        UpdateMesh();
    }

    public void Initialize()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshFilter.sharedMesh == null || meshFilter.sharedMesh.vertexCount != 24)
        {
            Mesh cubeMesh = CreateCubeMesh();
            meshFilter.sharedMesh = cubeMesh;
            originalVertices = cubeMesh.vertices;
        }
        else if (originalVertices == null || originalVertices.Length != 24)
        {
            originalVertices = meshFilter.sharedMesh.vertices;
        }

        if (meshRenderer.sharedMaterial == null)
        {
            meshRenderer.sharedMaterial = GetDefaultTransparentMaterial();
        }
        else if (overrideMaterial != null)
        {
            meshRenderer.sharedMaterial = overrideMaterial;
        }
    }

    public void UpdateMesh()
    {
        if (originalVertices == null || originalVertices.Length != 24)
            return;

        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] scaledVertices = new Vector3[24];

        Vector3 center = boxCollider.center;
        Vector3 size = boxCollider.size;

        for (int i = 0; i < 24; i++)
        {
            scaledVertices[i] = Vector3.Scale(originalVertices[i], size) + center;
        }

        mesh.vertices = scaledVertices;
        mesh.RecalculateBounds();
    }

    public void UpdateVisibility()
    {
        if (meshRenderer != null)
        {
            meshRenderer.enabled = visualize;
        }
    }

    private Mesh CreateCubeMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "GeneratedBoxMesh";

        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f,  0.5f),
            new Vector3( 0.5f, -0.5f,  0.5f),
            new Vector3( 0.5f,  0.5f,  0.5f),
            new Vector3(-0.5f,  0.5f,  0.5f),

            new Vector3( 0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f,  0.5f, -0.5f),
            new Vector3( 0.5f,  0.5f, -0.5f),

            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f,  0.5f),
            new Vector3(-0.5f,  0.5f,  0.5f),
            new Vector3(-0.5f,  0.5f, -0.5f),

            new Vector3( 0.5f, -0.5f,  0.5f),
            new Vector3( 0.5f, -0.5f, -0.5f),
            new Vector3( 0.5f,  0.5f, -0.5f),
            new Vector3( 0.5f,  0.5f,  0.5f),

            new Vector3(-0.5f,  0.5f,  0.5f),
            new Vector3( 0.5f,  0.5f,  0.5f),
            new Vector3( 0.5f,  0.5f, -0.5f),
            new Vector3(-0.5f,  0.5f, -0.5f),

            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3( 0.5f, -0.5f, -0.5f),
            new Vector3( 0.5f, -0.5f,  0.5f),
            new Vector3(-0.5f, -0.5f,  0.5f),
        };
        
        int[] triangles = new int[]
        {
            0, 1, 2, 0, 2, 3,         
            4, 5, 6, 4, 6, 7,      
            8, 9,10, 8,10,11,      
            12,13,14,12,14,15,        
            16,17,18,16,18,19,      
            20,21,22,20,22,23         
        };


        Vector2[] uvs = new Vector2[24];
        for (int i = 0; i < 6; i++)
        {
            uvs[i * 4 + 0] = new Vector2(0, 0);
            uvs[i * 4 + 1] = new Vector2(1, 0);
            uvs[i * 4 + 2] = new Vector2(1, 1);
            uvs[i * 4 + 3] = new Vector2(0, 1);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    private Material GetDefaultTransparentMaterial()
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.name = "BoxColliderPreview_Material";
        mat.color = new Color(1f, 0.5f, 0f, 0.25f);
        mat.SetFloat("_Mode", 3);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
        return mat;
    }
    
    public void RebuildObject()
    {
        Mesh cubeMesh = CreateCubeMesh();
        meshFilter.sharedMesh = cubeMesh;
        originalVertices = cubeMesh.vertices;

        UpdateMesh();
        UpdateVisibility();

    }
}
