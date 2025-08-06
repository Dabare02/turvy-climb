using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour
{
    // Cantidad de salientes agarrados.
    public int grippedHoldsAmount { get; private set; }
    // Indica si Player está agarrado a algún saliente.
    public bool isPlayerAttachedToWall
    {
        get { return grippedHoldsAmount > 0; }
    }
    public bool hasStamina { get; private set; }

    private PlayerMovement _movementHandler;
    private PlayerAttackHandler _attackHandler;

    private UnityEvent<MoveEnum, float, float> startFewHandsHolding;
    private UnityEvent<MoveEnum> stopFewHandsHolding;

    [Header("Player parts")]
    public List<DraggableHand> playerHands;
    public DraggableTorso playerTorso;
    [Header("Movement area ranges")]
    public float handMoveRange = 4.0f;
    public float torsoMoveRange = 5.0f;
    [Header("Hand Parameters")]
    [Tooltip("Rango normal de detección de salientes.")]
    public float regularHoldDetectRange = 1.5f;
    [Tooltip("Rango ampliado de detección de salientes, para situaciones como el uso de " +
        "Quick Drop o Slingshot.")]
    public float largeHoldDetectRange = 3.0f;
    [Tooltip("En el caso de inhabilitar el agarre de salientes de una mano, cuanto tiempo"
        + " debería durar esto?")]
    public float tempGripDisableDuration = 0.8f;

    [Header("Attack Parameters")]
    public PlayerAttackTypeSO punchAttack;
    public PlayerAttackTypeSO slingshotAttack;
    [Header("Stamina Cost Parameters")]
    public StaminaCostSO dragHandSTCost;
    public StaminaCostSO dragTorsoSTCost;
    public StaminaCostSO gripHoldSTCost;
    [Tooltip("Coste de aguante drenado al estar colgando de una sola extremidad.")]
    public StaminaCostSO singleHoldSTCost;
    [Tooltip("Coste de aguante drenado al estar colgando de dos extremidades.")]
    public StaminaCostSO doubleHoldSTCost;

    void Awake()
    {
        _movementHandler = GetComponent<PlayerMovement>();
        _attackHandler = GetComponent<PlayerAttackHandler>();

        for (int i = 0; i < playerHands.Count; i++)
        {
            PunchHandler punchHandler = playerHands[i].GetComponent<PunchHandler>();
            punchHandler.punchAttack = punchAttack;
        }

        hasStamina = true;

        // Suscripción de eventos.
        GameObject levelmngObj = GameObject.FindGameObjectWithTag("LevelManager");
        if (levelmngObj != null)
        {
            StaminaManager staminaManager = levelmngObj.GetComponent<StaminaManager>();

            startFewHandsHolding = new UnityEvent<MoveEnum, float, float>();
            stopFewHandsHolding = new UnityEvent<MoveEnum>();
            startFewHandsHolding.AddListener(staminaManager.StartContinuousStaminaDrain);
            stopFewHandsHolding.AddListener(staminaManager.StopContinuousStaminaChange);
        }
    }

    // Indica si la parte de cuerpo especificada puede ser agarrada con el ratón.
    public bool IsBodyPartGrabbable(DraggableBodyPart bodyPart)
    {
        if (bodyPart.GetType() == typeof(DraggableHand))
        {   // Si es una mano, debe haber al menos una mano agarrada a un saliente (aparte de si misma).
            DraggableHand hand = bodyPart.GetComponent<DraggableHand>();
            if ((hand.isGripped && grippedHoldsAmount < 2)
                || (!hand.isGripped && !isPlayerAttachedToWall)
                || !hasStamina)
            {
                return false;
            }
        }
        else
        {   // Si es el torso, el jugador debe de estar agarrado a, al menos, un saliente.
            if (!isPlayerAttachedToWall || !hasStamina)
            {
                return false;
            }
        }

        return true;
    }

    public void StartMovingBodyPart(Rigidbody2D movingPart)
    {
        if (hasStamina)
        {
            _movementHandler.StartMovingBodyPart(movingPart);
        }
    }

    public void StopMovingBodyPart()
    {
        _movementHandler.StopMovingBodyPart();
    }

    public void StartAttackDetection(DraggableBodyPart bodyPart)
    {
        _attackHandler.StartAttackDetection(bodyPart);
    }

    public void StopAttackDetection()
    {
        _attackHandler.StopAttackDetection();
    }

    public void CheckAttack()
    {
        if (hasStamina)
        {
            _attackHandler.CheckAttack();
        }
    }

    public void IncreaseGrippedHolds(int amount)
    {
        grippedHoldsAmount += amount;

        switch (grippedHoldsAmount)
        {
            case 1:
                startFewHandsHolding.Invoke(MoveEnum.FewHandsHolding, singleHoldSTCost.staminaCost, singleHoldSTCost.staminaChangeSpeed);
                break;
            case 2:
                startFewHandsHolding.Invoke(MoveEnum.FewHandsHolding, doubleHoldSTCost.staminaCost, doubleHoldSTCost.staminaChangeSpeed);
                break;
            default:
                stopFewHandsHolding.Invoke(MoveEnum.FewHandsHolding);
                break;
        }
        Debug.Log("Gripped holds: " + grippedHoldsAmount);
    }

    public void DecreaseGrippedHolds(int amount)
    {
        grippedHoldsAmount -= amount;

        switch (grippedHoldsAmount)
        {
            case 1:
                startFewHandsHolding.Invoke(MoveEnum.FewHandsHolding, singleHoldSTCost.staminaCost, singleHoldSTCost.staminaChangeSpeed);
                break;
            case 2:
                startFewHandsHolding.Invoke(MoveEnum.FewHandsHolding, doubleHoldSTCost.staminaCost, doubleHoldSTCost.staminaChangeSpeed);
                break;
            default:
                stopFewHandsHolding.Invoke(MoveEnum.FewHandsHolding);
                break;
        }
        Debug.Log("Gripped holds: " + grippedHoldsAmount);
    }

    // Soltar todos los salientes.
    public void DropAllHolds()
    {
        for (int i = 0; i < playerHands.Count; i++)
        {
            playerHands[i].DropHold();
        }
    }

    // Establecer el rango de detección de saliente de cada mano a la versión normal.
    public void SetRegularHoldDetectRange()
    {
        for (int i = 0; i < playerHands.Count; i++)
        {
            playerHands[i].SetRegularHoldDetectRange();
        }
    }
    // Establecer el rango de detección de saliente de cada mano a la versión ampliada.
    public void SetLargeHoldDetectRange()
    {
        for (int i = 0; i < playerHands.Count; i++)
        {
            playerHands[i].SetLargeHoldDetectRange();
        }
    }

    // Devuelve la primera mano que tenga un saliente en su rango de detección.
    public DraggableHand GetHandWithHoldInRange()
    {
        for (int i = 0; i < playerHands.Count; i++)
        {
            if (playerHands[i].holdInRange != null) return playerHands[i];
        }

        return null;
    }

    // Devuelve una mano aleatoria.
    public DraggableHand GetRandomHand()
    {
        List<DraggableHand> shuffledHands = playerHands.ToList();
        Utilities.Shuffle(shuffledHands);
        return shuffledHands[0];
    }

    // Devuelve una mano que tenga un saliente en su rango de detección de forma aleatoria.
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

    public void OutOfStamina()
    {
        hasStamina = false;
        DropAllHolds();
        StopAttackDetection();
        StopMovingBodyPart();
    }
}
