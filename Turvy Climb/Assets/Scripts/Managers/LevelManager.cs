using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

// Se encarga de funciones específicas del nivel, incluyendo la gestión del nivel de aguante
[RequireComponent(typeof(StaminaManager))]
[RequireComponent(typeof(LevelProgressManager))]
public class LevelManager : MonoBehaviour
{
    private const int LIMBS_AMOUNT = 4;

    private Player _player;
    public LevelDataSO level;
    [SerializeField] private AudioClip levelMusic;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject completionMenu;
    [SerializeField] private GameObject tutorialMenu;
    [Tooltip("Los salientes a los que estará agarrado Player al empezar el nivel.")]
    [SerializeField] private Hold[] startingHolds;

    private List<Collectable> radishes;

    public float timePlayed
    {
        get; private set;
    }
    public float levelProgress
    {
        get; private set;
    }
    public float recordLevelProgress
    {
        get; private set;
    }

    private bool _gameOver;
    private bool _completed;

    void Awake()
    {
        // Comprobar datos del nivel.
        if (level == null)
        {
            Debug.LogWarning("There is no level data, the level can't start!");
            gameObject.SetActive(false);
            GeneralManager.Instance.GoToLevelSelect();
        }

        // Obtener referencias a Player.
        _player = FindObjectOfType<Player>();
        if (_player == null)
        {
            Debug.LogWarning("There is no player, the level can't start!");
            gameObject.SetActive(false);
            GeneralManager.Instance.GoToLevelSelect();
        }

        // Comprobar rábanos
        radishes = new List<Collectable>();
        GameObject[] radishObjs = GameObject.FindGameObjectsWithTag("Radish");
        Array.Sort(radishObjs, (a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
        foreach (GameObject o in radishObjs)
        {
            radishes.Add(o.GetComponent<Collectable>());
        }
    }

    void Start()
    {
        // LECTURA DATOS
        timePlayed = 0;
        recordLevelProgress = level.progress;

        // Rábanos
        if (level.radishesCollected.Length != radishes.Count)
        {
            level.radishesCollected = new bool[radishes.Count];
            // ^Con esta acción, se resetea la cuenta de rábanos conseguidos en el nivel.
        }
        else
        {
            for (int i = 0; i < radishes.Count; i++)
            {
                // Se destruyen los rábanos ya recogidos anteriormente.
                if (level.radishesCollected[i])
                {
                    Destroy(radishes[i].gameObject);
                }
            }
        }

        // ACCIONES INICIO NIVEL
        GrabStartingHolds();
        GetComponent<StaminaManager>().LockStaminaChange(false);

        // MÚSICA
        if (levelMusic != null)
        {
            GeneralManager.Instance.audioManager.PlayMusic(levelMusic);
        }

        if (tutorialMenu != null && !level.dontShowTutorialAgain)
        {
            OpenTutorial(true);
        }
    }

    void Update()
    {
        if (!_completed && !_gameOver && (tutorialMenu == null || !tutorialMenu.activeInHierarchy) && Input.GetKeyDown(KeyCode.Escape))
        {
            OpenOptions(!GeneralManager.Instance.pause);
        }

        UpdateTime();
    }

    void OnValidate()
    {
        if (startingHolds.Length < 4)
        {
            Debug.LogWarning("Array startingHolds must have a size of " + LIMBS_AMOUNT + ".");
            Array.Resize(ref startingHolds, LIMBS_AMOUNT);
        }
    }

    void OnEnable()
    {
        EventManager.LevelProgressChanged += UpdateProgress;
        EventManager.DSAValueChanged += SetDSATutorial;
    }

    void OnDisable()
    {
        EventManager.LevelProgressChanged -= UpdateProgress;
        EventManager.DSAValueChanged -= SetDSATutorial;
    }

    public void OpenGameOver(bool cond)
    {
        GeneralManager.Instance.ButtonPressedSFX();
        GeneralManager.Instance.pause = cond;
        gameOverMenu.SetActive(cond);
    }

    public void OpenLevelComplete(bool cond)
    {
        GeneralManager.Instance.ButtonPressedSFX();
        GeneralManager.Instance.pause = cond;
        completionMenu.SetActive(cond);
    }

    public void OpenTutorial(bool cond)
    {
        if (!cond) GeneralManager.Instance.ButtonPressedSFX();
        GeneralManager.Instance.pause = cond;
        tutorialMenu.SetActive(cond);
    }

    public void SetDSATutorial(bool cond)
    {
        // El único dato de guardado cambiado durante el nivel por simplicidad.
        level.dontShowTutorialAgain = cond;
    }

    public void GrabStartingHolds()
    {
        for (int i = 0; i < LIMBS_AMOUNT; i++)
        {
            if (startingHolds[i] != null)
            {
                startingHolds[i].FirstGrip();
                _player.playerHands[i].GripHold(startingHolds[i]);
            }
        }
    }

    public void LevelComplete()
    {
        if (!_completed)
        {
            Debug.Log("LEVEL COMPLETE");
            _completed = true;

            // TODO

            OpenLevelComplete(true);
        }
    }

    public void GameOver()
    {
        if (!_gameOver)
        {
            Debug.Log("GAME OVER");
            _gameOver = true;

            StaminaManager stManager = GetComponent<StaminaManager>();
            stManager.DepleteStamina();

            OpenGameOver(true);
        }
    }

    private void UpdateTime()
    {
        float newTime = timePlayed + Time.deltaTime;
        if (Mathf.FloorToInt(newTime) > Mathf.FloorToInt(timePlayed))
        {
            EventManager.OnLevelTimePassed(timePlayed);
        }
        timePlayed = newTime;
    }

    public void UpdateProgress(float progress)
    {
        levelProgress = progress;
        if (progress >= recordLevelProgress)
        {
            recordLevelProgress = progress;
        }
        if (progress >= 1f)
        {
            // NIVEL COMPLETADO
            LevelComplete();
        }
    }

    private void LogStats()
    {
        // TIEMPOS
        level.totalPlayedTime += timePlayed;
        if (_completed && timePlayed >= level.recordTime)
        {
            level.recordTime = timePlayed;
            Debug.Log("New record!");
        }

        // PROGRESO
        level.progress = recordLevelProgress;

        // RÁBANOS
        for (int i = 0; i < radishes.Count; i++)
        {
            level.radishesCollected[i] = radishes[i] == null;
        }

        // ESTRELLAS
        // Estrella 1
        if (_completed)
        {
            level.stars[0] = true;
        }

        // Estrella 2
        bool allTrue = level.radishesCollected != null
            //&& level.radishesCollected.Length > 0
            && level.radishesCollected.All(v => v);
        if (allTrue)
        {
            level.stars[1] = true;
        }

        Debug.Log("Level data logued!"
            + "\nTime played: " + timePlayed
            + "\nTotal time played: " + level.totalPlayedTime
            + "\nRecord progress: " + level.progress
            + "\nRecord time: " + level.recordTime);
    }

    public void OpenOptions(bool cond)
    {
        GeneralManager.Instance.OpenOptions(cond);
    }

    public void RestartLevel()
    {
        LogStats();
        SaveLoadManager.SaveLevelData(level);
        GeneralManager.Instance.RestartLevel();
    }

    public void GoBackToLevelSelect()
    {
        LogStats();
        SaveLoadManager.SaveLevelData(level);
        GeneralManager.Instance.GoToLevelSelect();
    }
}
