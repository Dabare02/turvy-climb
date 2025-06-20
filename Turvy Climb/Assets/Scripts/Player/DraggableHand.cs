using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Representa una mano que se puede mover. Tambien se encarga de la detección de colisiones.
public class DraggableHand : DraggableBodyPart
{
    private GameObject grippedHold;

    // TEMP (Buscar mejor solución para establecer el saliente al que se empieza agarrado
    // al principio del nivel).
    private bool firstHold = true;

    public void GripHold(GameObject hold)
    {
        transform.position = hold.transform.position;
        _body.constraints = RigidbodyConstraints2D.FreezeAll;

        player.IncreaseGrippedHolds(1);
    }

    public void DropHold()
    {
        _body.constraints = RigidbodyConstraints2D.FreezeRotation;

        player.DecreaseGrippedHolds(1);
    }

    protected new void OnMouseDown()
    {
        if (grippedHold != null)
        {
            DropHold();
        }
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

        // TEMP
        if (firstHold)
        {
            GripHold(grippedHold);
            firstHold = false;
        }
        // TEMP
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
