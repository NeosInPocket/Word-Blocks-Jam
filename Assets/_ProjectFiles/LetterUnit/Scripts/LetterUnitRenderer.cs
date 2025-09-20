using System;
using System.Collections.Generic;
using System.Linq;
using _ProjectFiles.LetterUnit.Scripts;
using UnityEngine;

public class LetterUnitRenderer : MonoBehaviour
{
    [SerializeField] private Transform characterTransform;
    [SerializeField] private CharactersDictionary characters;
    [SerializeField] private GameObject rendererObject;
    [SerializeField] private Color characterUnavailableColor;
    [SerializeField] private Color unitNotVisibleColor;
    [SerializeField] private GameObject questionMarkElement; 
    [SerializeField] private Animator animator;
    [SerializeField] private List<ParticleSystem> particleSystems;
    [SerializeField] private GameObject vFXFeedback;
    private GameObject characterObject;
    private GameObject questionMarkObject;
    private Renderer _renderer;
    private Renderer _characterRenderer;
    private Color _defaultColor;

    public GameObject CharacterObject => characterObject;

    public void SetCharacter(string character, Color defaultColor)
    {
        _defaultColor = defaultColor;
        RemoveCharacter();
        SetUnitColor(defaultColor);
        SetParticlesColor(defaultColor);
        
        var characterData = characters.Characters.FirstOrDefault(x => x.character == character);
        characterObject = Instantiate(characterData.model, characterTransform.position, characterTransform.rotation, transform);
        var scale = characterTransform.localScale.x;
        var newScale = scale * characterObject.transform.localScale;
        characterObject.transform.localScale = newScale;
        _characterRenderer = characterObject.GetComponent<Renderer>();
        
        var rotation = characterTransform.rotation;
        rotation *= Quaternion.Euler(-109.88f, 0, 0);
        var position = characterTransform.position;
        position.y += 0.02f;
        
        questionMarkObject = Instantiate(questionMarkElement, position, rotation, transform);
        questionMarkObject.transform.localScale =  characterTransform.localScale.x * questionMarkObject.transform.localScale;
    }

    public void EnableFeedback()
    {
        vFXFeedback.SetActive(true);
    }
    
    private void SetParticlesColor(Color color)
    {
        foreach (var ps in particleSystems)
        {
            var psRenderer = ps.GetComponent<ParticleSystemRenderer>();
            var materialBlock = new MaterialPropertyBlock();
            
            psRenderer.GetPropertyBlock(materialBlock);
            materialBlock.SetColor("_AlbedoColor", color);
            psRenderer.SetPropertyBlock(materialBlock);
        }
    }
    
    public void SetCharacterVisibility(bool visible)
    {
        characterObject.SetActive(visible);
        questionMarkObject.SetActive(!visible);
         
        var eyesMpb = new MaterialPropertyBlock();
        _renderer.GetPropertyBlock(eyesMpb, 1);
        eyesMpb.SetInt("_Available", visible ? 1 : 0);
        _renderer.SetPropertyBlock(eyesMpb, 1);

        SetUnitColor(visible ? _defaultColor : unitNotVisibleColor);
        animator.SetBool("Active", visible);
    }

    public void SetCharacterAvailability(bool avaliable)
    {
        SetCharacterColor(avaliable ? Color.white : characterUnavailableColor);
    }
    
    private void RemoveCharacter()
    {
        if (characterObject == null) return;
        Destroy(characterObject.gameObject);
    }
    
    private void SetCharacterColor(Color color)
    {
        var mpb = new MaterialPropertyBlock();
        _characterRenderer.GetPropertyBlock(mpb);
        mpb.SetColor("_AlbedoColor", color);
        _characterRenderer.SetPropertyBlock(mpb);
    }
    
    private void SetUnitColor(Color color)
    {
        if (_renderer == null)
            _renderer = rendererObject.GetComponent<Renderer>();
        
        var mpb = new MaterialPropertyBlock();
        _renderer.GetPropertyBlock(mpb);
        mpb.SetColor("_AlbedoColor", color);
        _renderer.SetPropertyBlock(mpb);
    }
}