using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private const int LIMBS_AMOUNT = 4;

    [Tooltip("Los salientes a los que estará agarrado Player al empezar el nivel.")]
    [SerializeField] private Hold[] startingHolds;

    private Player _player;

    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        if (_player == null)
        {
            Debug.LogWarning("There is no player, the level can't start!");
            // TEMP: En el futuro, usar función de General Manager para cerrar el juego.
            gameObject.SetActive(false);
        }
    }

    void Start()
    {
        GrabStartingHolds();
    }

    void OnValidate()
    {
        if (startingHolds.Length < 4)
        {
            Debug.LogWarning("Array startingHolds must have a size of " + LIMBS_AMOUNT + ".");
            Array.Resize(ref startingHolds, LIMBS_AMOUNT);
        }
    }

    public void Restart()
    {
        
    }

    public void QuitLevel()
    {
        
    }

    public void GrabStartingHolds()
    {
        for (int i = 0; i < LIMBS_AMOUNT; i++)
        {
            _player.playerHands[i].GripHold(startingHolds[i]);
        }
    }
}
