using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using Vector2 = UnityEngine.Vector2;


// Se encarga estrictamente del movimiento del personaje por ratón. No se encarga de detección
// de colisiones (por ejemplo, con salientes).
[RequireComponent(typeof(Player))]
public class PlayerMovement : MonoBehaviour
{
    private Player _player;

    // Parámetros para el arrastre del objeto.
    private Rigidbody2D draggedPart;
    public bool isPartDragging
    {
        get; private set;
    }

    // Parámetros para el rango de movimiento para el objeto.
    private Vector2 _originalPos;
    private Vector2 _rangeCenterPos;
    private float _rangeRadius;
    private bool _isMouseInRange = true;

    void Start()
    {
        _player = GetComponent<Player>();
        _player.SetRegularHoldDetectRange();
    }

    void Update()
    {
        // MOVIMIENTO: Quick grip / drop.
        if (Input.GetKeyDown(KeyCode.Space)) QuickGripDrop();
    }

    void FixedUpdate()
    {
        // MOVIMIENTO: Agarrar y arrastrar mano o torso.
        if (!GeneralManager.Instance.pause && isPartDragging && draggedPart != null)
        {
            MoveBodyPart();
        }
    }

    void OnEnable()
    {
        EventManager.EnemyForcesDropBodyPart += DropBodyPart;
    }

    void OnDisable()
    {
        EventManager.EnemyForcesDropBodyPart -= DropBodyPart;
    }

    // MOVIMIENTO: Agarrar y arrastrar mano o torso.
    private void MoveBodyPart()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 newPos = mousePos;

        if (draggedPart != null) {
            // Se actualiza el centro del rango de movimiento.
            if (draggedPart.GetComponent<DraggableHand>() != null) _rangeCenterPos = _player.playerTorso.transform.position;

            _isMouseInRange = Utilities.IsPointInsideCircle(_rangeCenterPos, _rangeRadius, mousePos);
            if (!_isMouseInRange)
            {   // Si el ratón se encuentra FUERA del rango de movimiento.
                newPos = Utilities.LineThroughCircleCenterIntersec(_rangeCenterPos, _rangeRadius, mousePos);
            }

            // Mover objeto hacia dirección correcta con la velocidad adecuada.
            Vector2 direction = newPos - (Vector2)draggedPart.transform.position;
            draggedPart.velocity = direction * draggedPart.GetComponent<DraggableBodyPart>().dragSpeed;
        }
    }

    public void StartMovingBodyPart(Rigidbody2D bodyPart)
    {
        // Guardar objeto a arrastrar.
        draggedPart = bodyPart;

        // Posición original
        _originalPos = draggedPart.transform.position;

        // Rango y centro de área por la que se podrá mover el objeto.
        if (draggedPart.GetComponent<DraggableHand>() != null)
        {
            // No se asigna el centro en el caso de ser la mano porque este irá cambiando
            // conforme se mueva el cuerpo para seguir a la mano.
            _rangeRadius = _player.handMoveRange;

            // Indicar al manejador de ataques que empieze a detectar si se va a realizar ataque.
            _player.StartAttackDetection(draggedPart.GetComponent<DraggableHand>());

            // Evento para empezar a drenar aguante.
            EventManager.OnPartStartedMoving(MoveEnum.DragHand, _player.dragHandSTCost.staminaCost, _player.dragHandSTCost.staminaChangeSpeed);
        }
        else
        {
            //for (int i = 0; i < playerHands.Length; i++) { _rangeCenterPos += (Vector2)playerHands[i].transform.position; }; _rangeCenterPos /= playerHands.Length;
            _rangeCenterPos = _originalPos;
            _rangeRadius = _player.torsoMoveRange;

            // Indicar al manejador de ataques que empieze a detectar si se va a realizar ataque.
            _player.StartAttackDetection(draggedPart.GetComponent<DraggableTorso>());

            // Evento para empezar a drenar aguante.
            if (_player.grippedHoldsAmount < 4)
            {
                EventManager.OnPartStartedMoving(MoveEnum.DragTorso, _player.dragTorsoSTCost.staminaCost, _player.dragTorsoSTCost.staminaChangeSpeed);
            }
        }

        isPartDragging = true;

        Debug.Log("Moving " + draggedPart.name);
    }

    public void StopMovingBodyPart()
    {
        if (isPartDragging && draggedPart != null)
        {
            // Indicar finalización de arrastre.
            isPartDragging = false;

            // Evento para dejar de drenar aguante (y para recuperar aguante en caso de que se agarre un
            // saliente por primera vez).
            bool hasGrippedHold = false;
            DraggableBodyPart part = draggedPart.GetComponent<DraggableBodyPart>();
            if (part.GetType() == typeof(DraggableHand))
            {
                DraggableHand hand = draggedPart.GetComponent<DraggableHand>();
                // Parar drenaje continuo.
                EventManager.OnPartStoppedMoving(MoveEnum.DragHand);
                if (hand.isGripped && !hand.grippedHold.firstGrip)
                {   // Si se está agarrando un saliente y es la primera vez que se agarra.
                    EventManager.OnFirstTimeGrabbedHold(_player.firstGripHoldSTCost.staminaCost);
                    // Indicar que ya ha sido agarrado, para no volver a regenerar aguante la próxima vez.
                    hand.grippedHold.FirstGrip();
                    hasGrippedHold = true;
                }
            }
            else EventManager.OnPartStoppedMoving(MoveEnum.DragTorso);

            _originalPos = new Vector2(float.NaN, float.NaN);
            _rangeCenterPos = new Vector2(float.NaN, float.NaN);
            _rangeRadius = 0.0f;

            // Indicar al manejador de ataques que realize un ataque si está listo.
            // Condición debido a gasto accidental de aguante.
            if (!hasGrippedHold) _player.CheckAttack();

            Debug.Log("Stopped moving " + draggedPart.name);
            draggedPart = null;
        }
    }

    // MOVIMIENTO: Quick grip / drop.
    private void QuickGripDrop()
    {
        if (!_player.hasStamina) return;

        if (_player.isPlayerAttachedToWall)
        {
            // Se hace que todas las manos se suelten de sus salientes con rango incrementado.
            _player.DropAllHolds();
            _player.SetLargeHoldDetectRange();
            StopMovingBodyPart();
        }
        else
        {
            // Se selecciona una de las manos y se hace que se agarre a un saliente.
            // Orden de elección: LeftHand -> RightHand -> LeftFoot -> RightFoot
            DraggableHand selectedHand = _player.GetHighestHandWHoldInRange();
            if (selectedHand != null)
            {
                selectedHand.GripHold(selectedHand.ClosestHold());
            }

            // Se reestablece el rango de detección de salientes de todas las extremidades.
            _player.SetRegularHoldDetectRange();
        }
    }

    public void DropBodyPart(DraggableBodyPart bodyPart)
    {
        if (bodyPart != null && draggedPart != null
            && bodyPart.name == draggedPart.name)
        {
            DraggableHand hand = draggedPart.GetComponent<DraggableHand>();
            if (hand != null)
            {
                hand.TempDisableGrip(_player.tempGripDisableDuration);
            }

            _player.StopAttackDetection();
            StopMovingBodyPart();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (isPartDragging)
        {
            Gizmos.color = Color.red;

            if (_isMouseInRange) Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(_rangeCenterPos, _rangeRadius);
        }
    }
}
