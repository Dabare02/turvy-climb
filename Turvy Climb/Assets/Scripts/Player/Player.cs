using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour
{
    public int grippedHoldsAmount { get; private set; }
    public bool isPlayerAttachedToWall
    {
        get { return grippedHoldsAmount > 0; }
    }

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

    public void DropAllHolds()
    {
        for (int i = 0; i < playerHands.Count; i++)
        {
            DraggableHand hand = playerHands[i].GetComponent<DraggableHand>();
            hand.DropHold();
        }
    }

    public GameObject GetHandWithHoldInRange()
    {
        for (int i = 0; i < playerHands.Count; i++)
        {
            DraggableHand hand = playerHands[i].GetComponent<DraggableHand>();
            if (hand.holdInRange != null) return playerHands[i];
        }

        return null;
    }

    public GameObject GetRandomHand()
    {
        List<GameObject> shuffledHands = playerHands.ToList();
        Utilities.Shuffle(shuffledHands);
        return shuffledHands[0];
    }

    public GameObject GetRandomHandWithHoldInRange()
    {
        List<GameObject> shuffledHands = playerHands.ToList();
        Utilities.Shuffle(shuffledHands);

        for (int i = 0; i < shuffledHands.Count; i++)
        {
            DraggableHand hand = shuffledHands[i].GetComponent<DraggableHand>();
            if (hand.holdInRange != null) return shuffledHands[i];
        }

        return null;
    }
}
