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

    void Awake()
    {
        _movementHandler = GetComponent<PlayerMovement>();
        _attackHandler = GetComponent<PlayerAttackHandler>();

        // Asignar datos de ataque a cada elemento que interviene en dicho ataque.
        for (int i = 0; i < playerHands.Count; i++)
        {
            SpecificAttackHandler punchHandler = playerHands[i].GetComponent<PunchHandler>();
            punchHandler.attackData = punchAttack;
        }
        SpecificAttackHandler slingshotHandler = playerTorso.GetComponent<SlingshotHandler>();
        slingshotHandler.attackData = slingshotAttack;

        // Instanciación de eventos.
        if (startFewHandsHolding == null) startFewHandsHolding = new UnityEvent<MoveEnum, float, float>();
        if (stopFewHandsHolding == null) stopFewHandsHolding = new UnityEvent<MoveEnum>();
        if (_movementHandler.onPartMoveStart == null) stopFewHandsHolding = new UnityEvent<MoveEnum>();
        if (_movementHandler.onPartMoveStop == null) stopFewHandsHolding = new UnityEvent<MoveEnum>();
        if (_movementHandler.onFirstGrabHold == null) stopFewHandsHolding = new UnityEvent<MoveEnum>();
        if (_movementHandler.onSlingshotStopEvent == null) stopFewHandsHolding = new UnityEvent<MoveEnum>();
        if (_attackHandler.onPunch == null) stopFewHandsHolding = new UnityEvent<MoveEnum>();
        if (_attackHandler.onSlingshot == null) stopFewHandsHolding = new UnityEvent<MoveEnum>();

        hasStamina = true;
    }

    void OnEnable()
    {
        // Eventos.
        GameObject levelmngObj = GameObject.FindGameObjectWithTag("LevelManager");
        if (levelmngObj != null)
        {
            StaminaManager stManager = levelmngObj.GetComponent<StaminaManager>();

            // Subscripción a eventos.
            startFewHandsHolding.AddListener(stManager.StartContinuousStaminaDrain);
            stopFewHandsHolding.AddListener(stManager.StopContinuousStaminaChange);
            _movementHandler.onPartMoveStart.AddListener(stManager.StartContinuousStaminaDrain);
            _movementHandler.onPartMoveStop.AddListener(stManager.StopContinuousStaminaChange);
            _movementHandler.onFirstGrabHold.AddListener(stManager.IncreaseCurrentStamina);
            _movementHandler.onSlingshotStopEvent.AddListener(_attackHandler.SlingshotInterrupt);
            _attackHandler.onPunch.AddListener(levelmngObj.GetComponent<StaminaManager>().DecreaseCurrentStamina);
            _attackHandler.onSlingshot.AddListener(levelmngObj.GetComponent<StaminaManager>().DecreaseCurrentStamina);
        }
        GeneralManager.Instance.onPause.AddListener(StopAttackDetection);
    }

    void OnDisable()
    {
        startFewHandsHolding.RemoveAllListeners();
        stopFewHandsHolding.RemoveAllListeners();
        _movementHandler.onPartMoveStart.RemoveAllListeners();
        _movementHandler.onPartMoveStop.RemoveAllListeners();
        _movementHandler.onFirstGrabHold.RemoveAllListeners();
        _movementHandler.onSlingshotStopEvent.RemoveAllListeners();
        _attackHandler.onPunch.RemoveAllListeners();
        _attackHandler.onSlingshot.RemoveAllListeners();

        // ATTENTION: Could cause problems. If it does, check here and possibly change it
        // to `RemoveListener(_attackHandler.StopAttackDetection)`
        GeneralManager.Instance.onPause.RemoveAllListeners();
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
        DropAllHolds();
        StopAttackDetection();
        StopMovingBodyPart();
    }
}
