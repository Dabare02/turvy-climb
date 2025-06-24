using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Representa una mano que se puede mover. Tambien se encarga de la detección de colisiones.
public class DraggableHand : DraggableBodyPart
{
    public Hold holdInRange
    {
        get; private set;
    }

    // TEMP (Buscar mejor solución para establecer el saliente al que se empieza agarrado
    // al principio del nivel).
    private bool firstHold = true;

    public void GripHold(Hold hold)
    {
        if (hold != null && !hold.gripped)
        {
            //Debug.Log("Gripping hold " + hold.name);
            hold.Grip();

            transform.position = new Vector3(hold.transform.position.x, hold.transform.position.y, transform.position.z);
            _body.constraints = RigidbodyConstraints2D.FreezeAll;
            
            _player.IncreaseGrippedHolds(1);
        }
    }

    public void DropHold()
    {
        if (holdInRange != null)
        {
            //Debug.Log("Dropping hold " + holdInRange.name);
            _body.constraints = RigidbodyConstraints2D.FreezeRotation;
            _body.AddForce(Vector2.zero);   // Para forzar update. No recomendado, quizás usar corutina con "yield return new WaitForFixedUpdate()".

            holdInRange.UnGrip();

            _player.DecreaseGrippedHolds(1);
        }
    }

    protected new void OnMouseDown()
    {
        DropHold();

        base.OnMouseDown();
    }

    protected new void OnMouseUp()
    {
        GripHold(holdInRange);

        base.OnMouseUp();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        holdInRange = other.GetComponent<Hold>();
        if (holdInRange != null && !holdInRange.gripped)
        {
            Debug.Log("Hold in range.");

            // TEMP
            if (firstHold)
            {
                Debug.Log("First hold");
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
