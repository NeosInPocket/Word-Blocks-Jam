using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource clickSource;
    [SerializeField] private AudioMixer mixer;

    [Header("Clips")] 
    [SerializeField] private AudioClip music1;
    [SerializeField] private AudioClip music2;
    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip booster;
    [SerializeField] private AudioClip newWord;
    [SerializeField] private AudioClip wordCompleted;
    [SerializeField] private AudioClip win1;
    [SerializeField] private AudioClip win2;

    [Header("Clips")] 
    [SerializeField] private float minClickPitch;
    [SerializeField] private float maxClickPitch;

    private void Awake()
    {
        Instance = this;
        SaveManager.Load();

        SetMusicVolume(SaveManager.Data.MusicVolume);
        SetSFXVolume(SaveManager.Data.SFXVolume);
    }

    public void PlayMusic()
    {
        musicSource.clip = Random.Range(0, 2) == 0 ? music1 : music2;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayClick()
    {
        clickSource.pitch = Random.Range(minClickPitch, maxClickPitch);
        clickSource.PlayOneShot(click);
    }
    
    public void PlayBooster()
    {
        sfxSource.PlayOneShot(booster);
    }

    public void PlayWordCompleted()
    {
        sfxSource.PlayOneShot(wordCompleted);
    }

    public void PlayNewWord()
    {
        sfxSource.PlayOneShot(newWord);
    }

    public void PlayWin()
    {
        sfxSource.PlayOneShot(Random.Range(0, 2) == 0 ? win1 : win2);
    }

    public void SetMusicVolume(float volume)
    {
        var value = Mathf.Log10(volume) * 20;
        if (float.IsNegativeInfinity(value))
        {
            value = -80;
        }
        
        mixer.SetFloat("BGMVolume", value);
    }

    public void SetSFXVolume(float volume)
    {
        var value = Mathf.Log10(volume) * 20;
        if (float.IsNegativeInfinity(value))
        {
            value = -80;
        }
        
        mixer.SetFloat("SFXVolume", value);
    }
}