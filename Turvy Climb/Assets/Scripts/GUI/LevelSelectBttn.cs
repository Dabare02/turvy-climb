using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectBttn : MonoBehaviour
{
    private const int STAR_AMOUNT = 2;

    private LevelDataSO levelData;
    public BuildIndexes levelIndex
    {
        get; private set;
    }

    [SerializeField] private TMP_Text titleTMP;
    [SerializeField] private TMP_Text recordTimeTMP;
    [SerializeField] private Image[] stars;

    public void SetLevelIndex(BuildIndexes index)
    {
        levelIndex = index;
    }

    public void SetLevelData(LevelDataSO dataSO)
    {
        levelData = dataSO;

        titleTMP.text = "Nivel " + levelData.number + " - " + levelData.title;
        recordTimeTMP.text = Utilities.ConvertToMinutesTimerFormat(levelData.recordTime);
    }

    public void OnClick()
    {
        GeneralManager.Instance.LoadLevel(levelIndex);
    }
}
