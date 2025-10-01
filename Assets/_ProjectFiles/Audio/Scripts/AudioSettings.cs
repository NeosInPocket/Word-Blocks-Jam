public class AudioSettings
{
    private AudioManager audioManager;

    private float _musicVolume;
    private float _sfxVolume;

    public static AudioSettings Instance { get; private set; }
    
    public AudioSettings(AudioManager manager)
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            return;
        }
        
        audioManager = manager;

        SaveManager.Load();
        _musicVolume = SaveManager.Data.MusicVolume;
        _sfxVolume = SaveManager.Data.SFXVolume;

        ApplySettings();
    }

    public void SetMusicVolume(float volume)
    {
        _musicVolume = volume;
        audioManager.SetMusicVolume(_musicVolume);

        SaveManager.Data.MusicVolume = _musicVolume;
        SaveManager.Save();
    }

    public void SetSFXVolume(float volume)
    {
        _sfxVolume = volume;
        audioManager.SetSFXVolume(_sfxVolume);

        SaveManager.Data.SFXVolume = _sfxVolume;
        SaveManager.Save();
    }

    private void ApplySettings()
    {
        audioManager.SetMusicVolume(_musicVolume);
        audioManager.SetSFXVolume(_sfxVolume);
    }
}