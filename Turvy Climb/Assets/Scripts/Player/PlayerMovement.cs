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
    private Vector2 _originalPos;
    private bool _isMouseInRange = true;
    private Vector2 _rangeCenterPos;
    private float _rangeRadius;

    [SerializeField] private GameObject playerTorso;
    //[SerializeField] private GameObject[] playerHands;
    [SerializeField] public float handMoveRange = 4.0f;     // Rango de movimiento para la mano.
    [SerializeField] public float torsoMoveRange = 5.0f;   // Rango de movimiento para el torso.

    void Update()
    {
        if (_isPartDragging && draggedPart != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 newPos = mousePos;

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
        _isPartDragging = true;

        // Posición original
        _originalPos = draggedPart.transform.position;
        
        // Rango y centro de área por la que se podrá mover el objeto.
        if (draggedPart.CompareTag("Hand"))
        {
            _rangeCenterPos = playerTorso.transform.position;
            _rangeRadius = handMoveRange;
        }
        else
        {
            /*
            for (int i = 0; i < playerHands.Length; i++)
            {
                _rangeCenterPos += (Vector2)playerHands[i].transform.position;
            }
            _rangeCenterPos /= playerHands.Length;
            */
            _rangeCenterPos = _originalPos;
            _rangeRadius = torsoMoveRange;
        }

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

        _originalPos = new Vector2(float.NaN, float.NaN);
        _rangeCenterPos = new Vector2(float.NaN, float.NaN);
        _rangeRadius = 0.0f;

        draggedPart = null;
    }

    public void ResetPartPos()
    {
        // Devolver objeto a pos inicial si el ratón se encontraba en posición fuera de rango
        // cuando dejó de arrastrar.
        if (!_isMouseInRange) draggedPart.transform.position = _originalPos;
    }

    private void OnDrawGizmosSelected()
    {
        if (_isPartDragging)
        {
            Gizmos.color = Color.red;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (_isMouseInRange) Gizmos.color = Color.green;
            
            Gizmos.DrawWireSphere(_rangeCenterPos, _rangeRadius);
        }
    }
}
