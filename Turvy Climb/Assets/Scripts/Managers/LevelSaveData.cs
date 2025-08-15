[System.Serializable]
public class LevelSaveData
{
    public float recordTime;
    public int collectedRadishes;
    public int stars;

    public LevelSaveData(LevelDataSO level)
    {
        recordTime = level.recordTime;
        collectedRadishes = level.collectedRadishes;
        stars = level.stars;
    }
}
