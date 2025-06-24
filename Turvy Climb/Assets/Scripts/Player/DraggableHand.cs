using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

// Representa una mano que se puede mover. Tambien se encarga de la detección de colisiones.
public class DraggableHand : DraggableBodyPart
{
    public Hold holdInRange
    {
        get; private set;
    }

    public bool isGripped
    {
        get
        {
            if (holdInRange != null && holdInRange.gripped) return true;
            return false;
        }
    }

    // TEMP (Buscar mejor solución para establecer el saliente al que se empieza agarrado
    // al principio del nivel).
    private bool firstHold = true;

    [SerializeField] private CircleCollider2D holdDetector;

    public void SetRegularHoldDetectRange()
    {
        holdDetector.radius = _player.regularHoldDetectRange;
    }
    public void SetLargeHoldDetectRange()
    {
        holdDetector.radius = _player.largeHoldDetectRange;
    }

    public void GripHold(Hold hold)
    {
        if (hold != null && !hold.gripped)
        {
            Debug.Log("Gripping hold " + hold.name);
            // Indicar al saliente que hay una extremidad sujeta a este.
            hold.gripped = true;

            // Mover mano y congelar su RigidBody.
            transform.position = new Vector3(hold.transform.position.x, hold.transform.position.y, transform.position.z);
            _body.constraints = RigidbodyConstraints2D.FreezeAll;

            // Indicar a Player que hay un saliente más al que está agarrado.
            _player.IncreaseGrippedHolds(1);
        }
    }

    public void DropHold()
    {
        if (holdInRange != null && holdInRange.gripped)
        {
            Debug.Log("Dropping hold " + holdInRange.name);
            // Descongelar RigidBody.
            _body.constraints = RigidbodyConstraints2D.FreezeRotation;
            _body.AddForce(Vector2.zero);   // Para forzar update. No recomendado, quizás usar corutina con "yield return new WaitForFixedUpdate()".

            // Indicar al saliente que ya no hay extremiades sujetas a este.
            holdInRange.gripped = false;

            // Indicar a Player que hay un saliente menos al que está agarrado.
            _player.DecreaseGrippedHolds(1);
        }
    }

    protected new void OnMouseDown()
    {
        // Comprobar si hay al menos 1 mano agarrada a la pared (aparte de si misma).
        // Si no es el caso, no se permitirá cogerlo.
        if (!_player.IsBodyPartGrabbable(this)) return;

        DropHold();
        base.OnMouseDown();
    }

    protected new void OnMouseUp()
    {
        // Comprobar si hay al menos 1 mano agarrada a la pared (aparte de si misma).
        // Si no es el caso, no se permitirá cogerlo.
        if (!_player.IsBodyPartGrabbable(this)) return;

        GripHold(holdInRange);
        base.OnMouseUp();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Hold holdScr = other.GetComponent<Hold>();
        if (holdScr != null && !holdScr.gripped)
        {
            Debug.Log("Hold in range.");
            holdInRange = holdScr;

            // TEMP
            if (firstHold)
            {
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
