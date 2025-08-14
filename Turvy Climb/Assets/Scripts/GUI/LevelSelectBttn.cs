using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectBttn : MonoBehaviour
{
    public BuildIndexes levelIndex
    {
        get; private set;
    }

    public void SetLevelIndex(BuildIndexes index)
    {
        levelIndex = index;
    }

    public void OnClick()
    {
        GeneralManager.Instance.LoadLevel(levelIndex);
    }
}
