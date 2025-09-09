using UnityEngine;
using UnityEngine.UI;

public class SettingsView : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    private AudioSettings audioSettings;

    private void Start()
    {
        audioSettings = new AudioSettings(audioManager);
        SaveManager.Load();
        musicSlider.value = SaveManager.Data.MusicVolume;
        sfxSlider.value = SaveManager.Data.SFXVolume;
    }

    public void OnMusicVolumeChanged(float volume)
    {
        audioSettings.SetMusicVolume(volume);
    }
    
    public void OnSfxVolumeChanged(float volume)
    {
        audioSettings.SetSFXVolume(volume);
    }
}
