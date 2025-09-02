using System;
using System.Linq;
using _ProjectFiles.LetterUnit.Scripts;
using UnityEngine;

public class LetterUnitCharacterRenderer : MonoBehaviour
{
    [SerializeField] private Transform characterTransform;
    [SerializeField] private CharactersDictionary characters;
    [SerializeField] private GameObject rendererObject;
    private GameObject characterObject;
    private Renderer _renderer;

    public GameObject CharacterObject => characterObject;

    public void SetCharacter(string character)
    {
        RemoveCharacter();
        
        var characterData = characters.Characters.FirstOrDefault(x => x.character == character);
        characterObject = Instantiate(characterData.model, characterTransform.position, characterTransform.rotation, transform);
        var scale = characterTransform.localScale.x;
        var newScale = scale * characterObject.transform.localScale;
        characterObject.transform.localScale = newScale;
    }

    public void SetCharacterVisibility(bool visible)
    {
        characterObject.SetActive(visible);
    }
    
    private void RemoveCharacter()
    {
        if (characterObject == null) return;
        Destroy(characterObject.gameObject);
    }
    
    public void SetUnitColor(Color color)
    {
        if (_renderer == null)
            _renderer = rendererObject.GetComponent<Renderer>();
        
        var mpb = new MaterialPropertyBlock();
        _renderer.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", color);
        _renderer.SetPropertyBlock(mpb);
    }
}