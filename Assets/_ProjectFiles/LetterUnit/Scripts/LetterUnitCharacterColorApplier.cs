using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LetterUnitCharacterColorApplier : MonoBehaviour
{
    [SerializeField] private Color color;
    private List<Renderer> renderers;
    
    private void GetRenderers()
    {
        renderers = FindObjectsByType<LetterUnitCharacterRenderer>(FindObjectsSortMode.None).Select(x => x.CharacterObject.GetComponent<Renderer>()).ToList();
    }

    private void Update()
    {
        if (renderers == null || renderers.Count == 0)
            GetRenderers();

        foreach (var rend in renderers)
        {
            var propBlock = new MaterialPropertyBlock();
            
            rend.GetPropertyBlock(propBlock);
            propBlock.SetColor("_Color", color);
            rend.SetPropertyBlock(propBlock);
        }
    }
}