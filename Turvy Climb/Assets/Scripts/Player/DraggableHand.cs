using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Representa una mano que se puede mover. Tambien se encarga de la detección de colisiones.
public class DraggableHand : DraggableBodyPart
{
    private GameObject grippedHold;

    public void GripHold(GameObject hold)
    {
        transform.position = grippedHold.transform.position;
        _body.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    protected new void OnMouseDown()
    {
        _body.constraints = RigidbodyConstraints2D.FreezeRotation;
        base.OnMouseDown();
    }

    protected new void OnMouseUp()
    {
        if (grippedHold != null)
        {
            GripHold(grippedHold);
        }

        base.OnMouseUp();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hold"))
        {
            grippedHold = other.gameObject;
            Debug.Log("Hold in range.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Hold") && (grippedHold == other.gameObject))
        {
            grippedHold = null;
            Debug.Log("Hold left range.");
        }
    }
}
