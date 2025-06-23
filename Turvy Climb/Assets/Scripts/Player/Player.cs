using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour
{
    public int grippedHoldsAmount { get; private set; }

    private PlayerMovement _movementHandler;

    [Header("Player parts")]
    public List<GameObject> playerHands;
    public GameObject playerTorso;
    [Header("Movement area ranges")]
    public float handMoveRange = 4.0f;
    public float torsoMoveRange = 5.0f;

    void Start()
    {
        _movementHandler = GetComponent<PlayerMovement>();
    }

    public void StartMovingBodyPart(Rigidbody2D movingPart)
    {
        _movementHandler.StartMovingBodyPart(movingPart);
    }

    public void StopMovingBodyPart()
    {
        _movementHandler.StopMovingBodyPart();
    }

    public void IncreaseGrippedHolds(int amount)
    {
        grippedHoldsAmount += amount;
        Debug.Log("Gripped holds: " + grippedHoldsAmount);
    }

    public void DecreaseGrippedHolds(int amount)
    {
        grippedHoldsAmount -= amount;
        Debug.Log("Gripped holds: " + grippedHoldsAmount);
    }

    public GameObject GetRandomHand()
    {
        List<GameObject> shuffledHands = playerHands.ToList();
        Utilities.Shuffle(shuffledHands);
        return shuffledHands[0];
    }
}
