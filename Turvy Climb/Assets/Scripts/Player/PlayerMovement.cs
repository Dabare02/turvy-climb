using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using UnityEditor.Experimental.GraphView;


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
    private bool _isMouseInRange = true;
    private Vector2 _rangeCenterPos;
    private float _rangeRadius;

    void Start()
    {
        _player = GetComponent<Player>();
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
            if (draggedPart.CompareTag("Hand")) _rangeCenterPos = _player.playerTorso.transform.position;

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
        if (draggedPart.CompareTag("Hand"))
        {
            // No se asigna el centro en el caso de ser la mano porque este irá cambiando
            // conforme se mueva el cuerpo para seguir a la mano.
            _rangeRadius = _player.handMoveRange;
        }
        else
        {
            //for (int i = 0; i < playerHands.Length; i++) { _rangeCenterPos += (Vector2)playerHands[i].transform.position; }; _rangeCenterPos /= playerHands.Length;
            _rangeCenterPos = _originalPos;
            _rangeRadius = _player.torsoMoveRange;
        }

        _isPartDragging = true;

        Debug.Log("Moving " + draggedPart.name);
    }

    public void StopMovingBodyPart()
    {
        // Indicar finalización de arrastre.
        _isPartDragging = false;

        _originalPos = new Vector2(float.NaN, float.NaN);
        _rangeCenterPos = new Vector2(float.NaN, float.NaN);
        _rangeRadius = 0.0f;
        
        Debug.Log("Stopped moving " + draggedPart.name);
        draggedPart = null;
    }

    // MOVIMIENTO: Quick grip / drop.
    private void QuickGripDrop()
    {
        if (_player.isPlayerAttachedToWall)
        {
            // Se hace que todas las manos se suelten de sus salientes con rango incrementado.
            _player.DropAllHolds();
            _player.SetLargeHoldDetectRange();
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
