using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Se encarga de funciones específicas del nivel, incluyendo la gestión del nivel de aguante
[RequireComponent(typeof(StaminaManager))]
public class LevelManager : MonoBehaviour
{
    private const int LIMBS_AMOUNT = 4;

    public LevelDataSO level;
    [Tooltip("Los salientes a los que estará agarrado Player al empezar el nivel.")]
    [SerializeField] private Hold[] startingHolds;

    private Player _player;

    private bool _gameOver;

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
    }

    void Start()
    {
        GrabStartingHolds();
        GetComponent<StaminaManager>().LockStaminaChange(false);
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

    public void GameOver()
    {
        // TODO: Funcionalidad Game Over (pantalla de game over, volver a main menu con funcion en general manager...)
        if (!_gameOver)
        {
            // 
            Debug.Log("GAME OVER");
            _gameOver = true;
            StaminaManager stManager = GetComponent<StaminaManager>();
            stManager.DepleteStamina();

            GoBackToLevelSelect();
        }
    }

    public void GoBackToLevelSelect()
    {
        SaveLoadManager.SaveLevelData(level);
        GeneralManager.Instance.GoToLevelSelect();
    }
}
