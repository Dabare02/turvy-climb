using System.Collections.Generic;

[System.Serializable]
public class LevelSaveData
{
    public float totalPlayedTime;
    public float recordTime;
    public float levelProgress;
    public bool dontShowTutorialAgain;
    public bool[] stars;
    public bool[] radishesCollected;

    public LevelSaveData(LevelDataSO level)
    {
        totalPlayedTime = level.totalPlayedTime;
        recordTime = level.recordTime;
        levelProgress = level.progress;
        dontShowTutorialAgain = level.dontShowTutorialAgain;
        stars = level.stars;
        radishesCollected = level.radishesCollected;
    }
}
