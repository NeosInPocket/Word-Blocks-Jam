using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")] 
    [SerializeField] private AudioClip music1;
    [SerializeField] private AudioClip music2;
    [SerializeField] private AudioClip click1;
    [SerializeField] private AudioClip click2;

    private void Awake()
    {
        Instance = this;
        SaveManager.Load();
        musicSource.volume = SaveManager.Data.MusicVolume;
        sfxSource.volume = SaveManager.Data.SFXVolume;
    }

    public void PlayMusic()
    {
        musicSource.clip = Random.Range(0, 2) == 0 ? music1 : music2;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayClick()
    {
        sfxSource.PlayOneShot(Random.Range(0, 2) == 0 ? click1 : click2);
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}