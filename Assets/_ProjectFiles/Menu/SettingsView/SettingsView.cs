using UnityEngine;
using UnityEngine.UI;

public class SettingsView : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        SaveManager.Load();
        musicSlider.value = SaveManager.Data.MusicVolume;
        sfxSlider.value = SaveManager.Data.SFXVolume;
    }
    
    public void OnMusicVolumeChanged(float volume)
    {
        AudioSettings.Instance.SetMusicVolume(volume);
    }
    
    public void OnSfxVolumeChanged(float volume)
    {
        AudioSettings.Instance.SetSFXVolume(volume);
    }
}
