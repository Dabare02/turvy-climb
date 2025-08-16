using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

// Se encarga de funciones específicas del nivel, incluyendo la gestión del nivel de aguante
[RequireComponent(typeof(StaminaManager))]
public class LevelManager : MonoBehaviour
{
    private const int LIMBS_AMOUNT = 4;

    private Player _player;
    public LevelDataSO level;
    [SerializeField] private AudioClip levelMusic;
    [Tooltip("Los salientes a los que estará agarrado Player al empezar el nivel.")]
    [SerializeField] private Hold[] startingHolds;

    public float timePlayed
    {
        get; private set;
    }
    public float levelProgress
    {
        get; private set;
    }

    private bool _gameOver;
    private bool _completed;

    public UnityEvent<float> onSecondPassed;

    void Awake()
    {
        if (level == null)
        {
            Debug.LogWarning("There is no level data, the level can't start!");
            gameObject.SetActive(false);
            GeneralManager.Instance.GoToLevelSelect();
        }

        _player = FindObjectOfType<Player>();
        if (_player == null)
        {
            Debug.LogWarning("There is no player, the level can't start!");
            gameObject.SetActive(false);
            GeneralManager.Instance.GoToLevelSelect();
        }

        if (onSecondPassed == null) onSecondPassed = new UnityEvent<float>();
    }

    void Start()
    {
        timePlayed = 0;

        GrabStartingHolds();
        GetComponent<StaminaManager>().LockStaminaChange(false);

        if (levelMusic != null)
        {
            GeneralManager.Instance.audioManager.PlayMusic(levelMusic);
        }
    }

    void Update()
    {
        if (!_completed && !_gameOver && Input.GetKeyDown(KeyCode.Escape))
        {
            GeneralManager.Instance.pause = !GeneralManager.Instance.pause;
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
                _player.playerHands[i].GripHold(startingHolds[i]);
            }
        }
    }

    public void LevelComplete()
    {
        
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

            GoBackToLevelSelect();
        }
    }

    #region TimePlayed
    private void UpdateTime()
    {
        float newTime = timePlayed + Time.deltaTime;
        if (newTime != timePlayed)
        {
            timePlayed = newTime;
            onSecondPassed.Invoke(timePlayed);
        }
    }

    private void LogTime()
    {
        // TODO: Cambiar para que no guarde tiempo record si no lo completas.
        level.totalPlayedTime += timePlayed;
        if (_completed && timePlayed >= level.recordTime)
        {
            level.recordTime = timePlayed;
            Debug.Log("New record!");
        }

        Debug.Log("Level data logued!"
            + "\nTime played: " + timePlayed
            + "\nTotal time played: " + level.totalPlayedTime
            + "\nRecord time: " + level.recordTime);
    }
    #endregion

    #region LevelProgress
    public void UpdateProgress(float progress)
    {
        levelProgress = progress;
    }
    #endregion

    public void RestartLevel()
    {
        LogTime();
        SaveLoadManager.SaveLevelData(level);
        GeneralManager.Instance.RestartLevel();
    }

    public void GoBackToLevelSelect()
    {
        LogTime();
        SaveLoadManager.SaveLevelData(level);
        GeneralManager.Instance.GoToLevelSelect();
    }
}
