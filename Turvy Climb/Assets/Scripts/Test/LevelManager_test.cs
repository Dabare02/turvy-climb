using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

// Se encarga de funciones específicas del nivel, incluyendo la gestión del nivel de aguante
[RequireComponent(typeof(StaminaManager))]
[RequireComponent(typeof(LevelProgressManager_test))]
public class LevelManager_test : MonoBehaviour
{
    private const int LIMBS_AMOUNT = 4;

    private Player _player;
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

    void Awake()
    {
        _player = FindObjectOfType<Player>();
        if (_player == null)
        {
            Debug.LogWarning("There is no player, the level can't start!");
            gameObject.SetActive(false);
            GeneralManager.Instance.GoToLevelSelect();
        }
    }

    void Start()
    {
        timePlayed = 0;

        GrabStartingHolds();
        GetComponent<StaminaManager>().LockStaminaChange(false);
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
        if (!_completed)
        {
            Debug.Log("LEVEL COMPLETE");
            _completed = true;

            // TODO
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
        }
    }

    private void UpdateTime()
    {
        timePlayed += Time.deltaTime;
    }

    public void UpdateProgress(float progress)
    {
        levelProgress = progress;
        if (progress >= 1f)
        {
            // NIVEL COMPLETADO
            LevelComplete();
        }
    }
}
