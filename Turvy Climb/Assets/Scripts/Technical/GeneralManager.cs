using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioManager))]
public class GeneralManager : Singleton<GeneralManager>
{
    [Header("Componentes básicos.")]
    public AudioManager audioManager;
    [SerializeField] GameObject optionsPanel;

    public float transitionTime = 3f;

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
                optionsPanel.SetActive(true);
            }
            else
            {
                Time.timeScale = 1;
                optionsPanel.SetActive(false);
            }
        }
    }

    public override void Awake()
    {
        base.Awake();
        audioManager = GetComponent<AudioManager>();
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

    public void GoToMainMenu()
    {
        SceneManager.LoadScene((int)BuildIndexes.MainMenu);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public int GetNumberOfLevels()
    {
        return SceneManager.sceneCountInBuildSettings - 3;
    }
}
