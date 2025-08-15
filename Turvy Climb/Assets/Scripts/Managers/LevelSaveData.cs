using System.Collections.Generic;

[System.Serializable]
public class LevelSaveData
{
    public float totalPlayedTime;
    public float recordTime;
    public Dictionary<int, bool> radishesCollected;
    public Dictionary<int, bool> stars;

    public LevelSaveData(LevelDataSO level)
    {
        totalPlayedTime = level.totalPlayedTime;
        recordTime = level.recordTime;
        radishesCollected = level.radishesCollected;
        stars = level.stars;
    }
}
