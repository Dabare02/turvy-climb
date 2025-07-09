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
    private bool _isPartDragging;

    // Parámetros para el rango de movimiento para el objeto.
    private Vector2 _originalPos;
    private Vector2 _rangeCenterPos;
    private float _rangeRadius;
    private bool _isMouseInRange = true;

    public UnityEvent<MoveEnum, float, float> onPartMoveStart;
    public UnityEvent<MoveEnum> onPartMoveStop;

    void Start()
    {
        _player = GetComponent<Player>();

        GameObject levelManagerObj = GameObject.FindGameObjectWithTag("LevelManager");
        if (levelManagerObj != null)
        {
            StaminaManager stManager = levelManagerObj.GetComponent<StaminaManager>();

            onPartMoveStart.AddListener(stManager.StartContinuousStaminaDrain);
            onPartMoveStop.AddListener(stManager.StopContinuousStaminaChange);
        }
    }

    void Update()
    {
        // MOVIMIENTO: Agarrar y arrastrar mano o torso.
        MoveBodyPart();

        // MOVIMIENTO: Quick grip / drop.
        if (Input.GetKeyDown(KeyCode.Space)) QuickGripDrop();
    }

    // MOVIMIENTO: Agarrar y arrastrar mano o torso.
    private void MoveBodyPart()
    {
        if (_isPartDragging && draggedPart != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 newPos = mousePos;

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
            onPartMoveStart.Invoke(MoveEnum.DragHand, _player.dragHandSTCost.staminaCost, _player.dragHandSTCost.staminaChangeSpeed);
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
                onPartMoveStart.Invoke(MoveEnum.DragTorso, _player.dragTorsoSTCost.staminaCost, _player.dragTorsoSTCost.staminaChangeSpeed);
            }
        }

        _isPartDragging = true;

        Debug.Log("Moving " + draggedPart.name);
    }

    public void StopMovingBodyPart()
    {
        Debug.Log("Is part moving? " + _isPartDragging + ". Is draggedPart null? " + (draggedPart == null));
        if (_isPartDragging && draggedPart != null)
        {
            // Indicar finalización de arrastre.
            _isPartDragging = false;

            // Evento para dejar de drenar aguante.
            if (draggedPart.GetComponent<DraggableHand>() != null) onPartMoveStop.Invoke(MoveEnum.DragHand);
            else onPartMoveStop.Invoke(MoveEnum.DragTorso);

            _originalPos = new Vector2(float.NaN, float.NaN);
            _rangeCenterPos = new Vector2(float.NaN, float.NaN);
            _rangeRadius = 0.0f;

            // Indicar al manejador de ataques que realize un ataque si está listo.
            _player.CheckAttack();

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
            DraggableHand selectedHand = _player.GetHandWithHoldInRange();
            if (selectedHand != null)
            {
                selectedHand.GripHold(selectedHand.holdInRange);
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
        if (_isPartDragging)
        {
            Gizmos.color = Color.red;

            if (_isMouseInRange) Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(_rangeCenterPos, _rangeRadius);
        }
    }
}
