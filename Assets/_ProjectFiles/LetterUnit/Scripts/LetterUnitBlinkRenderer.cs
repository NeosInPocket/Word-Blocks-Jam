using System.Collections;
using UnityEngine;

public class LetterUnitBlinkRenderer : MonoBehaviour
{
    [SerializeField] private GameObject rendererObject;
    [SerializeField] private LetterUnitRenderer letterUnitRenderer;
    [SerializeField] [Range(0f, 1f)] private float darkenFactor = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float lightenFactor = 0.5f;
    [SerializeField] private float blinkDuration = 0.5f;

    private Renderer _renderer;
    private MaterialPropertyBlock _block;

    private Color _blinkColor;

    public void Initialize(Color color)
    {
        _renderer = rendererObject.GetComponent<Renderer>();
        _block = new MaterialPropertyBlock();
        _renderer.GetPropertyBlock(_block);
    }

    public void Blink(bool darken)
    {
        StopAllCoroutines();

        var propertyBlock = new MaterialPropertyBlock();
        _renderer.GetPropertyBlock(propertyBlock);
        var originalColor = letterUnitRenderer.CurrentColor;
        
        if (darken)
        {
            _blinkColor = originalColor * darkenFactor;
        }
        else
        {
            _blinkColor = Color.Lerp(originalColor, Color.white, lightenFactor);
        }

        _blinkColor.a = originalColor.a;

        StartCoroutine(BlinkCoroutine());
    }

    private IEnumerator BlinkCoroutine()
    {
        float elapsed = 0f;
        while (elapsed < blinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / blinkDuration;
            Color currentColor = Color.Lerp(_blinkColor, letterUnitRenderer.CurrentColor, t);
            _renderer.GetPropertyBlock(_block);
            _block.SetColor("_AlbedoColor", currentColor);
            _renderer.SetPropertyBlock(_block);
            yield return null;
        }

        _renderer.GetPropertyBlock(_block);
        _block.SetColor("_AlbedoColor", letterUnitRenderer.CurrentColor);
        _renderer.SetPropertyBlock(_block);
    }
}