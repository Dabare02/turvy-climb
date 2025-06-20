using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using UnityEditor.Experimental.GraphView;

public class PlayerMovement : MonoBehaviour
{
    // Parámetros para el arrastre del objeto.
    private Rigidbody2D draggedPart;
    private bool _isPartDragging;
    // Parámetros para el rango de movimiento para el objeto.
    private Vector2 originalPos;
    private bool isMouseInRange = true;

    [SerializeField] private GameObject playerBody;
    [SerializeField] public float handMoveRange = 30.0f;    // Rango de movimiento para el objeto.

    void Update()
    {
        if (_isPartDragging && draggedPart != null)
        {
            /*
            Vector3 camPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            draggedPart.transform.position = new Vector3(camPos.x, camPos.y, draggedPart.transform.position.z);
            */

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 newPos = mousePos;

            isMouseInRange = Utilities.IsPointInsideCircle(playerBody.transform.position, handMoveRange, mousePos);
            if (!isMouseInRange)
            {   // Si el ratón se encuentra FUERA del rango de manos/pies.
                newPos = Utilities.LineThroughCircleCenterIntersec(playerBody.transform.position, handMoveRange, mousePos);
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
        ResetPartPos();

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
            if (Utilities.IsPointInsideCircle(playerBody.transform.position, handMoveRange, mousePos))
            {
                Gizmos.color = Color.green;
            }

            Gizmos.DrawWireSphere(playerBody.transform.position, handMoveRange);
        }
    }
}
