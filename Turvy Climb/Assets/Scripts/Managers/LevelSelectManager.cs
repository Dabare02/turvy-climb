using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelSelectManager : MonoBehaviour
{
    [Header("Botones de nivel")]
    [SerializeField] private GameObject lvlSelectBttnPrefab;
    [SerializeField] private GameObject lvlSelectBttnLockedPrefab;
    [SerializeField] private GameObject lvlSelectBttnNotImplPrefab;
    [SerializeField] private GameObject levelSelectContainer;

    [Header("Estadísticas")]
    [SerializeField] private TMP_Text totalPlayedTimeTMP;
    [SerializeField] private TMP_Text radishesCollectedTMP;

    [Header("Otros")]
    [SerializeField] private AudioClip levelSelectTheme;

    void Start()
    {
        // Level buttons
        List<LevelDataSO> levels = GeneralManager.Instance.levels;
        BuildIndexes[] indexes = (BuildIndexes[])Enum.GetValues(typeof(BuildIndexes));
        
        for (int i = 0; i < GeneralManager.Instance.GetNumberOfLevels(); i++)
        {
            // Si es el primer nivel.
            if (i == 0)
            {
                GameObject firstLvlBttn = Instantiate(lvlSelectBttnPrefab, levelSelectContainer.transform);
                firstLvlBttn.GetComponent<LevelSelectBttn>().SetLevelData(levels[0], false);
                firstLvlBttn.GetComponent<LevelSelectBttn>().SetLevelIndex(indexes[GeneralManager.Instance.GetFirstLevelIndex()]);
                continue;
            }

            // Para cualquier otro nivel, primero comprobar que esté implementado.
            if (!levels[i].implemented)
            {
                Instantiate(lvlSelectBttnNotImplPrefab, levelSelectContainer.transform);
            }
            else if (levels[i - 1].progress < 1f)
            {
                GameObject lockedBttn = Instantiate(lvlSelectBttnLockedPrefab, levelSelectContainer.transform);
                lockedBttn.GetComponent<LevelSelectBttn>().SetLevelData(levels[0], true);
            }
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

        // Música
        if (levelSelectTheme != null) GeneralManager.Instance.audioManager.PlayMusic(levelSelectTheme);
    }

    public void GoBackToMainMenu()
    {
        GeneralManager.Instance.GoToMainMenu();
    }
}
