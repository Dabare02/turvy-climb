using System.Collections.Generic;
using System.Linq;
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

    [SerializeField] private Image previewIMG;
    [SerializeField] private TMP_Text titleTMP;
    [SerializeField] private TMP_Text progressTMP;
    [SerializeField] private TMP_Text recordTimeTMP;
    [SerializeField] private Image[] stars;
    [SerializeField] private Sprite emptyStar;
    [SerializeField] private Sprite fullStar;

    [Header("Colores")]
    [SerializeField] private Color notPlayedTextColor;
    [SerializeField] private Color playedTextColor;
    [SerializeField] private Color completedColor;

    public void SetLevelIndex(BuildIndexes index)
    {
        levelIndex = index;
    }

    public void SetLevelData(LevelDataSO dataSO, bool locked)
    {
        levelData = dataSO;

        if (levelData.preview != null)
        {
            previewIMG.sprite = levelData.preview;
        }
        titleTMP.text = "Nivel " + (levelData.number + 1) + " - " + levelData.title;

        if (!locked)
        {
            progressTMP.text = Mathf.FloorToInt(levelData.progress * 100) + "%";
            switch (levelData.progress)
            {
                case >= 1f:
                    progressTMP.color = completedColor;
                    recordTimeTMP.color = playedTextColor;
                    break;
                case <= 0f:
                    progressTMP.color = notPlayedTextColor;
                    recordTimeTMP.color = notPlayedTextColor;
                    break;
                default:
                    progressTMP.color = playedTextColor;
                    recordTimeTMP.color = playedTextColor;
                    break;
            }
            recordTimeTMP.text = Utilities.ConvertToMinutesTimerFormat(levelData.recordTime);

            // Estrellas obtenidas.
            for (int i = 0; i < stars.Count() && i < levelData.stars.Length; i++)
            {
                if (levelData.stars[i])
                {
                    stars[i].sprite = fullStar;
                }
                else
                {
                    stars[i].sprite = emptyStar;
                }
            }
        }
    }

    public void OnClick()
    {
        GeneralManager.Instance.LoadLevel(levelIndex);
    }
}
