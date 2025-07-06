using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Se encarga de funciones específicas del nivel, incluyendo la gestión del nivel de aguante
public class LevelManager : MonoBehaviour
{
    private const int LIMBS_AMOUNT = 4;

    [Tooltip("Los salientes a los que estará agarrado Player al empezar el nivel.")]
    [SerializeField] private Hold[] startingHolds;

    private Player _player;

    void Awake()
    {
        GameObject plObj = GameObject.FindGameObjectWithTag("Player");
        if (plObj == null)
        {
            Debug.LogWarning("There is no player, the level can't start!");
            gameObject.SetActive(false);
            GeneralManager.Instance.Quit();
        }
        else
        {
            _player = plObj.GetComponent<Player>();
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

    public void GrabStartingHolds()
    {
        for (int i = 0; i < LIMBS_AMOUNT; i++)
        {
            _player.playerHands[i].GripHold(startingHolds[i]);
        }
    }
}
