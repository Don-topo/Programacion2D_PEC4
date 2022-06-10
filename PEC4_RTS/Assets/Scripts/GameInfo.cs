using System;


[Serializable]
public class GameInfo
{
    public float generalVolume;
    public float effectsVolume;
    public string currentLevel;

    public GameInfo(float newGeneralVolume, float newEffectsVolume, string newCurrentLevel)
    {
        generalVolume = newGeneralVolume;
        effectsVolume = newEffectsVolume;
        currentLevel = newCurrentLevel;
    }
}
