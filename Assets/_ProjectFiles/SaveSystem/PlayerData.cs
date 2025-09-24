using System;

[Serializable]
public class PlayerData
{
    public float MusicVolume = 1f;
    public float SFXVolume = 1f;
    public bool hapticsEnabled = true;
    public int levelsCompleted = 0;
    public bool tutorialCompleted = false;
    public int unlockBoostersCount = 1;
    public int extraCellBoostersCount = 1;
}