using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using TMPro;

public class LevelSelectManager : MonoBehaviour
{
    [Header("Botones de nivel")]
    [SerializeField] private GameObject levelSelectButtonPrefab;
    [SerializeField] private GameObject levelSelectContainer;

    [Header("Estadísticas")]
    [SerializeField] private TMP_Text totalPlayedTimeTMP;
    [SerializeField] private TMP_Text radishesCollectedTMP;

    [Header("Música")]
    [SerializeField] private AudioClip levelSelectTheme;

    void Start()
    {
        // Level buttons
        List<LevelDataSO> levels = GeneralManager.Instance.levels;
        BuildIndexes[] indexes = (BuildIndexes[])Enum.GetValues(typeof(BuildIndexes));

        for (int i = 0; i < GeneralManager.Instance.GetNumberOfLevels(); i++)
        {
            GameObject bttn = Instantiate(levelSelectButtonPrefab, levelSelectContainer.transform);
            bttn.GetComponent<LevelSelectBttn>().SetLevelData(levels[i]);
            bttn.GetComponent<LevelSelectBttn>().SetLevelIndex(indexes[GeneralManager.Instance.GetFirstLevelIndex() + i]);
        }

        // Stats
        float totalPlayedTime = 0f;
        int radishesCollected = 0;
        foreach (LevelDataSO l in levels)
        {
            totalPlayedTime += l.totalPlayedTime;
            foreach (bool cond in l.radishesCollected)
            {
                if (cond) radishesCollected++;
            }
        }

        totalPlayedTimeTMP.text = Utilities.ConvertToMinutesTimerFormat(totalPlayedTime);
        radishesCollectedTMP.text = radishesCollected.ToString();

        // Musica
        if (levelSelectTheme != null) GeneralManager.Instance.audioManager.PlayMusic(levelSelectTheme);
    }

    public void GoBackToMainMenu()
    {
        GeneralManager.Instance.GoToMainMenu();
    }
}
