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
    [Tooltip("Los salientes a los que estará agarrado Player al empezar el nivel.")]
    [SerializeField] private Hold[] startingHolds;

    private List<Collectible> radishes;

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

    public UnityEvent<float> onSecondPassed;

    void Awake()
    {
        // Comprobar datos de nivel.
        if (level == null)
        {
            Debug.LogWarning("There is no level data, the level can't start!");
            gameObject.SetActive(false);
            GeneralManager.Instance.GoToLevelSelect();
        }

        // Obtener referencia a player.
        _player = FindObjectOfType<Player>();
        if (_player == null)
        {
            Debug.LogWarning("There is no player, the level can't start!");
            gameObject.SetActive(false);
            GeneralManager.Instance.GoToLevelSelect();
        }

        // Comprobar rábanos
        /*if (radishes == null || radishes.Length == 0)
        {
            Debug.LogWarning("Remember to include each radish in the array \"radishes\" in the inspector before running the level, if there is any.");
            radishes = new Collectible[] { };
        }*/

        // Hecho de esta manera para no tener que añadir los rábanos en el inspector por cada nivel.
        // El orden de FindGameObjectsWithTag es inconsistente, y dicho orden es importante para comproba qué rábanos se han conseguido.
        // Por ello, usamos Array.Sort con una expresión lambda para definir un IComparer que los ordene por su indice de hijo de su objeto padre (Sibling Index).
        radishes = new List<Collectible>();
        GameObject[] radishObjs = GameObject.FindGameObjectsWithTag("Radish");
        Array.Sort(radishObjs, (a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
        foreach (GameObject o in radishObjs)
        {
            radishes.Add(o.GetComponent<Collectible>());
        }

        if (onSecondPassed == null) onSecondPassed = new UnityEvent<float>();
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
    }

    void Update()
    {
        if (!_completed && !_gameOver && Input.GetKeyDown(KeyCode.Escape))
        {
            GeneralManager.Instance.OpenOptions(!GeneralManager.Instance.pause);
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

    void OnDisable()
    {
        onSecondPassed.RemoveAllListeners();
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

            GeneralManager.Instance.pause = true;
            completionMenu.SetActive(true);
        }
    }

    public void GameOver()
    {
        // TODO: Funcionalidad Game Over (pantalla de game over, volver a main menu con funcion en general manager...)
        if (!_gameOver)
        {
            Debug.Log("GAME OVER");
            _gameOver = true;

            StaminaManager stManager = GetComponent<StaminaManager>();
            stManager.DepleteStamina();

            GeneralManager.Instance.pause = true;
            gameOverMenu.SetActive(true);
        }
    }
    private void UpdateTime()
    {
        float newTime = timePlayed + Time.deltaTime;
        if (Mathf.FloorToInt(newTime) > Mathf.FloorToInt(timePlayed))
        {
            onSecondPassed.Invoke(timePlayed);
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
            LevelComplete();
        }
    }

    private void LogStats()
    {
        // Tiempos
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
            if (radishes[i] == null)
            {
                Debug.Log("Radish " + i + " collected");
                level.radishesCollected[i] = true;
            }
        }

        // ESTRELAS
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
