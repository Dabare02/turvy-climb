using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioManager))]
public class GeneralManager : Singleton<GeneralManager>
{
    [Header("Componentes básicos.")]
    public AudioManager audioManager;
    [SerializeField] GameObject optionsPanel;

    [Header("Parámetros generales")]
    public float transitionTime = 3f;
    // Player data
    [Tooltip("Si existen datos guardados, se ignorará este valor.")]
    public float maxPlayerStamina;
    public  StaminaCostSO radishSTCost;
    [NonSerialized] public float totalTimePlayed;

    [Header("Niveles")]
    public List<LevelDataSO> levels;

    public UnityEvent onPause;
    public UnityEvent onUnPause;

    private bool paused = false;
    public bool pause
    {
        get
        {
            return paused;
        }
        set
        {
            paused = value;
            if (paused)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
                optionsPanel.SetActive(false);
                onPause.Invoke();
            }
        }
    }

    public override void Awake()
    {
        base.Awake();
        audioManager = GetComponent<AudioManager>();

        for (int i = 0; i < levels.Count; i++)
        {
            LevelSaveData lvlSaveData = SaveLoadManager.LoadLevelData(i);
            if (lvlSaveData != null)
            {
                levels[i].totalPlayedTime = lvlSaveData.totalPlayedTime;
                levels[i].recordTime = lvlSaveData.recordTime;
                levels[i].progress = lvlSaveData.levelProgress;
                levels[i].stars = lvlSaveData.stars;
                levels[i].radishesCollected = lvlSaveData.radishesCollected;

                // Calcular y asignar max stamina.
                if (radishSTCost != null)
                {
                    maxPlayerStamina += radishSTCost.maxStaminaIncreaseAmount * levels[i].radishesCollected.Count(v => v);
                }
                else
                {
                    Debug.LogError("Give a reference to STCostRaddish to General Manager, or else the amount of max stamina won't be able to be calculated.");
                }
            }
            else
            {
                levels[i].totalPlayedTime = 0f;
                levels[i].recordTime = 0f;
                levels[i].progress = 0f;
                levels[i].stars = new bool[] { false, false };
                levels[i].radishesCollected = new bool[] { };   // Depende del nivel, por lo que su contenido real no se decidirá hasta que se cargue el nivel.
            }
        }
    }

    public void GoToNextLevel(float waitTime = -1)
    {
        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            Debug.Log("Last level finished.");
        }
        else
        {
            Debug.Log("Loading next level...");
            StartCoroutine(WaitAndLoadNextScene(waitTime));
        }
    }

    private IEnumerator WaitAndLoadNextScene(float waitSeconds)
    {
        if (waitSeconds < 0)
        {
            waitSeconds = transitionTime;
        }
        yield return new WaitForSeconds(waitSeconds);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadScene(BuildIndexes sceneIndex)
    {
        SceneManager.LoadScene((int)sceneIndex);
    }

    public void LoadLevel(BuildIndexes sceneIndex)
    {
        if (sceneIndex >= BuildIndexes.LevelOne)
        {
            SceneManager.LoadScene((int)sceneIndex);
        }
        else
        {
            Debug.LogError("The scene " + sceneIndex.HumanName() + " (index: " + (int)sceneIndex + ") is not a level!");
        }
    }

    public void OpenOptions(bool cond)
    {
        pause = cond;
        optionsPanel.SetActive(pause);
        if (pause) onPause.Invoke();
        else onUnPause.Invoke();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene((int)BuildIndexes.MainMenu);
    }

    public void GoToLevelSelect()
    {
        SceneManager.LoadScene((int)BuildIndexes.LevelSelectMenu);
    }

    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public int GetNumberOfLevels()
    {
        return levels.Count;
    }

    public int GetFirstLevelIndex()
    {
        return (int)BuildIndexes.LevelOne;
    }
}
