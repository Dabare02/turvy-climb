using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Representa una mano que se puede mover. Tambien se encarga de la detección de colisiones.
public class DraggableHand : DraggableBodyPart
{
    private Hold holdInRange;

    // TEMP (Buscar mejor solución para establecer el saliente al que se empieza agarrado
    // al principio del nivel).
    private bool firstHold = true;

    public void GripHold(Hold hold)
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
        if (holdInRange != null)
        {
            DropHold();
            holdInRange.UnGrip();
        }
        base.OnMouseDown();
    }

    protected new void OnMouseUp()
    {
        if (holdInRange != null && !holdInRange.gripped)
        {
            holdInRange.Grip();
            GripHold(holdInRange);
        }

        base.OnMouseUp();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Hold holdScr = other.GetComponent<Hold>();
        if (holdScr != null && !holdScr.gripped)
        {
            holdInRange = holdScr;
            Debug.Log("Hold in range.");

            // TEMP
            if (firstHold)
            {
                holdInRange.Grip();
                GripHold(holdInRange);
                firstHold = false;
            }
            // TEMP
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Hold holdScr = other.GetComponent<Hold>();
        if (holdScr != null && holdScr.gameObject == other.gameObject)
        {
            holdInRange = null;
            Debug.Log("Hold left range.");
        }
    }
}
