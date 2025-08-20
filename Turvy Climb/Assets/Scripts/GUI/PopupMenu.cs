using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupMenu : MonoBehaviour
{
    //public void OnResume()
    //{
    //    GeneralManager.Instance.pause = false;
    //    gameObject.SetActive(false);
    //}

    public void OnGoToLevelSelect()
    {
        GeneralManager.Instance.pause = false;

        LevelManager lvlManager = FindObjectOfType<LevelManager>();
        if (lvlManager != null) lvlManager.GoBackToLevelSelect();
        else
        {
            GeneralManager.Instance.GoToLevelSelect();
            Debug.LogError("LevelManager not found.");
        }
    }

    public void OnRestartLevel()
    {
        GeneralManager.Instance.pause = false;
        
        LevelManager lvlManager = FindObjectOfType<LevelManager>();
        if (lvlManager != null) lvlManager.RestartLevel();
        else
        {
            GeneralManager.Instance.RestartLevel();
            Debug.LogError("LevelManager not found.");
        }
    }
}
