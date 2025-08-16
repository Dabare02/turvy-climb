using System.Collections.Generic;

[System.Serializable]
public class LevelSaveData
{
    public float totalPlayedTime;
    public float recordTime;
    public float levelProgress;
    public bool[] stars;
    public bool[] radishesCollected;

    public LevelSaveData(LevelDataSO level)
    {
        totalPlayedTime = level.totalPlayedTime;
        recordTime = level.recordTime;
        levelProgress = level.progress;
        stars = level.stars;
        radishesCollected = level.radishesCollected;
    }
}
