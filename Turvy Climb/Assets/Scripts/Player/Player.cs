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
    private PlayerAttackHandler _attackHandler;

    [Header("Player parts")]
    public List<DraggableHand> playerHands;
    public DraggableTorso playerTorso;
    [Header("Movement area ranges")]
    public float handMoveRange = 4.0f;
    public float torsoMoveRange = 5.0f;
    [Header("Hand Parameters")]
    public float regularHoldDetectRange = 1.5f;
    public float largeHoldDetectRange = 3.0f;

    [Header("Attack Parameters")]
    public PlayerAttackTypeSO punchAttack;
    public PlayerAttackTypeSO slingshotAttack;

    void Awake()
    {
        _movementHandler = GetComponent<PlayerMovement>();
        _attackHandler = GetComponent<PlayerAttackHandler>();
    }

    void Start()
    {
        for (int i = 0; i < playerHands.Count; i++)
        {
            PunchHandler punchHandler = playerHands[i].GetComponent<PunchHandler>();
            punchHandler.punchAttack = punchAttack;
        }
    }

    public bool IsBodyPartGrabbable(DraggableBodyPart bodyPart)
    {
        if (bodyPart.CompareTag("Hand"))
        {
            DraggableHand hand = bodyPart.GetComponent<DraggableHand>();
            if ((hand.isGripped && grippedHoldsAmount < 2)
                || (!hand.isGripped && !isPlayerAttachedToWall))
            {
                return false;
            }
        }
        else
        {
            if (!isPlayerAttachedToWall)
            {
                return false;
            }
        }

        return true;
    }

    public void StartMovingBodyPart(Rigidbody2D movingPart)
    {
        _movementHandler.StartMovingBodyPart(movingPart);
    }

    public void StopMovingBodyPart()
    {
        _movementHandler.StopMovingBodyPart();
    }

    public void StartAttackDetection(DraggableBodyPart bodyPart)
    {
        _attackHandler.StartAttackDetection(bodyPart);
    }

    public void CheckAttack()
    {
        _attackHandler.CheckAttack();
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

    // Soltar todos los salientes. Si increasedRange = true, las manos tendrán mayor rango de agarre
    // al soltarse (utilizado principalmente para movimientos como QuickDrop).
    public void DropAllHolds()
    {
        for (int i = 0; i < playerHands.Count; i++)
        {
            playerHands[i].DropHold();
        }
    }

    public void SetRegularHoldDetectRange()
    {
        for (int i = 0; i < playerHands.Count; i++)
        {
            playerHands[i].SetRegularHoldDetectRange();
        }
    }
    public void SetLargeHoldDetectRange()
    {
        for (int i = 0; i < playerHands.Count; i++)
        {
            playerHands[i].SetLargeHoldDetectRange();
        }
    }

    public DraggableHand GetHandWithHoldInRange()
    {
        for (int i = 0; i < playerHands.Count; i++)
        {
            if (playerHands[i].holdInRange != null) return playerHands[i];
        }

        return null;
    }

    public DraggableHand GetRandomHand()
    {
        List<DraggableHand> shuffledHands = playerHands.ToList();
        Utilities.Shuffle(shuffledHands);
        return shuffledHands[0];
    }

    public DraggableHand GetRandomHandWithHoldInRange()
    {
        List<DraggableHand> shuffledHands = playerHands.ToList();
        Utilities.Shuffle(shuffledHands);

        for (int i = 0; i < shuffledHands.Count; i++)
        {
            if (shuffledHands[i].holdInRange != null) return playerHands[i];
        }

        return null;
    }
}
