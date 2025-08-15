using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class LevelSelectManager : MonoBehaviour
{
    [SerializeField] private GameObject levelSelectButtonPrefab;
    [SerializeField] private GameObject levelSelectContainer;

    void Start()
    {
        List<LevelDataSO> levels = GeneralManager.Instance.levels;
        BuildIndexes[] indexes = (BuildIndexes[])Enum.GetValues(typeof(BuildIndexes));

        for (int i = 0; i < GeneralManager.Instance.GetNumberOfLevels(); i++)
        {
            GameObject bttn = Instantiate(levelSelectButtonPrefab, levelSelectContainer.transform);
            bttn.GetComponent<LevelSelectBttn>().SetLevelData(levels[i]);
            bttn.GetComponent<LevelSelectBttn>().SetLevelIndex(indexes[GeneralManager.Instance.GetFirstLevelIndex() + i]);
        }
    }

    public void GoBackToMainMenu()
    {
        GeneralManager.Instance.GoToMainMenu();
    }
}
