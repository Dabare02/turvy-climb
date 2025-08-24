using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Audio;
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
    [Tooltip("La longitud de la linea que indica la trayectoria de los ataques")]
    public float trajectoryLineLength = 20f;
    [Header("Stamina Cost Parameters")]
    public StaminaCostSO dragHandSTCost;
    public StaminaCostSO dragTorsoSTCost;
    [Tooltip("Aguante regenerado al agarrarse a un saliente al que el jugador no se" +
        " ha agarrado previamente.")]
    public StaminaCostSO firstGripHoldSTCost;
    [Tooltip("Coste de aguante drenado al estar colgando de una sola extremidad.")]
    public StaminaCostSO singleHoldSTCost;
    [Tooltip("Coste de aguante drenado al estar colgando de dos extremidades.")]
    public StaminaCostSO doubleHoldSTCost;

    [Header("Sounds")]
    public AudioClip gripHoldSFX;
    public AudioClip hurtSFX;

    void Awake()
    {
        _movementHandler = GetComponent<PlayerMovement>();
        _attackHandler = GetComponent<PlayerAttackHandler>();

        // Asignar datos a manos.
        for (int i = 0; i < playerHands.Count; i++)
        {
            SpecificAttackHandler punchHandler = playerHands[i].GetComponent<PunchHandler>();
            punchHandler.attackData = punchAttack;

            DraggableHand hand = playerHands[i].GetComponent<DraggableHand>();
            hand.Setup(gripHoldSFX);
        }

        // Asignar datos a torso.
        SpecificAttackHandler slingshotHandler = playerTorso.GetComponent<SlingshotHandler>();
        slingshotHandler.attackData = slingshotAttack;

        hasStamina = true;
    }

    void Update()
    {
        RotateHands();
    }

    void OnEnable()
    {
        EventManager.PausedManually += StopMovingBodyPart;
        EventManager.StaminaDepleted += OutOfStamina;
    }

    void OnDisable()
    {
        EventManager.PausedManually -= StopMovingBodyPart;
        EventManager.StaminaDepleted -= OutOfStamina;
    }

    // FUNCIONES PRIVADAS
    private void RotateHands()
    {
        foreach (DraggableHand h in playerHands)
        {
            Rigidbody2D handRb = h.GetComponent<Rigidbody2D>();
            Vector3 dir = (h.transform.position - playerTorso.transform.position).normalized;

            //h.transform.rotation = Quaternion.FromToRotation(h.transform.up, dir);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            handRb.rotation = angle;
        }
    }

    // FUNCIONES PUBLICAS

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

        Debug.Log("SI HAY STAMINA; SI SE PUEDE AGARRAR!");
        return true;
    }

    public bool IsABodyPartMoving()
    {
        return _movementHandler.isPartDragging;
    }

    public void StartMovingBodyPart(Rigidbody2D movingPart)
    {
        if (hasStamina && !GeneralManager.Instance.pause)
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
                EventManager.OnStartFewHandsHolding(MoveEnum.FewHandsHolding, singleHoldSTCost.staminaCost, singleHoldSTCost.staminaChangeSpeed);
                break;
            case 2:
                EventManager.OnStartFewHandsHolding(MoveEnum.FewHandsHolding, doubleHoldSTCost.staminaCost, doubleHoldSTCost.staminaChangeSpeed);
                break;
            default:
                EventManager.OnStopFewHandsHolding(MoveEnum.FewHandsHolding);
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
                EventManager.OnStartFewHandsHolding(MoveEnum.FewHandsHolding, singleHoldSTCost.staminaCost, singleHoldSTCost.staminaChangeSpeed);
                break;
            case 2:
                EventManager.OnStartFewHandsHolding(MoveEnum.FewHandsHolding, doubleHoldSTCost.staminaCost, doubleHoldSTCost.staminaChangeSpeed);
                break;
            default:
                EventManager.OnStopFewHandsHolding(MoveEnum.FewHandsHolding);
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

    // Activar/desactivar habilidad de poder agarrar salientes.

    // Devuelve la primera mano que tenga un saliente en su rango de detección.
    public DraggableHand GetHandWithHoldInRange()
    {
        for (int i = 0; i < playerHands.Count; i++)
        {
            if (playerHands[i].holdsInRange.Count > 0) return playerHands[i];
        }

        return null;
    }

    // Devuelve la mano con un saliente en su rango de detección que se encuentre más arriba.
    public DraggableHand GetHighestHandWHoldInRange()
    {
        DraggableHand hand = playerHands[0];
        for (int i = 0; i < playerHands.Count; i++)
        {
            if (playerHands[i].holdsInRange.Count > 0
                && playerHands[i].transform.position.y
                > hand.transform.position.y)
            {
                hand = playerHands[i];
            }
        }

        return hand;
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
            if (shuffledHands[i].holdsInRange.Count > 0) return shuffledHands[i];
        }

        return null;
    }

    /* FUNCIONES TÉCNICAS */
    public void EnableBodyColliders(bool cond)
    {
        for (int i = 0; i < playerHands.Count; i++)
        {
            playerHands[i].GetComponent<CircleCollider2D>().enabled = cond;
        }
        playerTorso.GetComponent<CircleCollider2D>().enabled = cond;
    }

    public void ChangeSpringJointsFrequency(float newFrequency = -1f)
    {
        playerTorso.ChangeSpringFrequency(newFrequency);
    }

    public void ChangeSpringJointsDistance(float newDistance = -1f)
    {
        playerTorso.ChangeSpringDistance(newDistance);
    }

    public void ChangeSpringJointsDampRatio(float newDamp = -1f)
    {
        playerTorso.ChangeSpringDampenRatio(newDamp);
    }

    public void ActivateGravity(bool cond)
    {
        float gravScale = cond ? 1f : 0f;

        for (int i = 0; i < playerHands.Count; i++)
        {
            playerHands[i].GetComponent<Rigidbody2D>().gravityScale = gravScale;
        }
        playerTorso.GetComponent<Rigidbody2D>().gravityScale = gravScale;
    }

    public void ActivateEnemyInmunity(bool cond)
    {
        string layerName = cond ? "PlayerEnemyPassthrough" : "Player";

        for (int i = 0; i < playerHands.Count; i++)
        {
            playerHands[i].gameObject.layer = LayerMask.NameToLayer(layerName);
        }
        playerTorso.gameObject.layer = LayerMask.NameToLayer(layerName);
    }

    public void OutOfStamina()
    {
        hasStamina = false;

        foreach (DraggableHand h in playerHands) {
            h.lockedDrag = true;
        }
        playerTorso.lockedDrag = true;
        
        DropAllHolds();
        StopAttackDetection();
        StopMovingBodyPart();
    }
}
