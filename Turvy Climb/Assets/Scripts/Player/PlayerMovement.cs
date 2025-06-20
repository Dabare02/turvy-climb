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
public class PlayerMovement : MonoBehaviour
{
    // Parámetros para el arrastre del objeto.
    private Rigidbody2D draggedPart;
    private bool _isPartDragging;
    // Parámetros para el rango de movimiento para el objeto.
    private Vector2 originalPos;
    private bool isMouseInRange = true;

    [SerializeField] private GameObject playerTorso;
    [SerializeField] public float handMoveRange = 4.0f;     // Rango de movimiento para la mano.
    [SerializeField] public float torsoMoveRange = 5.0f;   // Rango de movimiento para el torso.

    void Update()
    {
        if (_isPartDragging && draggedPart != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 newPos = mousePos;

            Vector2 rangeCenterPos = Vector2.zero;
            float rangeRadius = 0.0f;

            // Determinar el centro y radio del área donde se permite mover la parte de
            // cuerpo agarrada.
            if (draggedPart.CompareTag("Hand"))
            {
                rangeCenterPos = playerTorso.transform.position;
                rangeRadius = handMoveRange;
            }
            else
            {
                rangeCenterPos = originalPos;
                rangeRadius = torsoMoveRange;
            }

            isMouseInRange = Utilities.IsPointInsideCircle(rangeCenterPos, rangeRadius, mousePos);
            if (!isMouseInRange)
            {   // Si el ratón se encuentra FUERA del rango de movimiento.
                newPos = Utilities.LineThroughCircleCenterIntersec(rangeCenterPos, rangeRadius, mousePos);
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
        _isPartDragging = true;

        // Guardar pos inicial del objeto.
        originalPos = draggedPart.transform.position;

        Debug.Log("Moving " + draggedPart.name);
    }

    public void StopMovingBodyPart()
    {
        Debug.Log("Stopped moving " + draggedPart.name);
        // Indicar finalización de arrastre.
        _isPartDragging = false;

        // TODO: Comportamiento temporal.
        // Aquí irá función comprobando que hacer según donde se suelte el objeto.
        //ResetPartPos();

        originalPos = new Vector2(float.NaN, float.NaN);
        draggedPart = null;
    }

    public void ResetPartPos()
    {
        // Devolver objeto a pos inicial si el ratón se encontraba en posición fuera de rango
        // cuando dejó de arrastrar.
        if (!isMouseInRange) draggedPart.transform.position = originalPos;
    }

    private void OnDrawGizmosSelected()
    {
        if (_isPartDragging)
        {
            Gizmos.color = Color.red;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (isMouseInRange) Gizmos.color = Color.green;
            
            if (draggedPart.CompareTag("Hand"))
            {
                Gizmos.DrawWireSphere(playerTorso.transform.position, handMoveRange);
            }
            else
            {
                Gizmos.DrawWireSphere(originalPos, torsoMoveRange);
            }
        }
    }
}
