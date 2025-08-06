using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

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
    public bool gripEnabled
    {
        get; private set;
    }

    [SerializeField] private CircleCollider2D holdDetector;

    new void Awake()
    {
        base.Awake();

        gripEnabled = true;
    }

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
        if (gripEnabled && hold != null && !hold.gripped)
        {
            // Indicar al saliente que hay una extremidad sujeta a este.
            hold.gripped = true;

            // Mover mano y congelar su RigidBody.
            transform.position = new Vector3(hold.transform.position.x, hold.transform.position.y, transform.position.z);
            _body.constraints = RigidbodyConstraints2D.FreezeAll;

            // Establecer nuevo saliente como holdInRange (en caso de que el trigger falle).
            if (holdInRange == null) holdInRange = hold;

            // Indicar a Player que hay un saliente más al que está agarrado.
            _player.IncreaseGrippedHolds(1);
            
            Debug.Log(this.name + " is gripping hold " + holdInRange.name);
        }
    }

    public void DropHold()
    {
        if (holdInRange != null && holdInRange.gripped)
        {
            Debug.Log(this.name + " is dropping hold " + holdInRange.name);
            // Descongelar RigidBody.
            _body.constraints = RigidbodyConstraints2D.FreezeRotation;
            _body.AddForce(Vector2.zero);   // Para forzar update. No recomendado, quizás usar corutina con "yield return new WaitForFixedUpdate()".

            // Indicar al saliente que ya no hay extremiades sujetas a este.
            holdInRange.gripped = false;

            // Indicar a Player que hay un saliente menos al que está agarrado.
            _player.DecreaseGrippedHolds(1);
        }
    }

    public void TempDisableGrip(float seconds)
    {
        StartCoroutine(DisableGripCoroutine(seconds));
    }

    private IEnumerator DisableGripCoroutine(float seconds)
    {
        gripEnabled = false;
        yield return new WaitForSeconds(seconds);
        gripEnabled = true;
    }

    protected override void OnMouseDown()
    {
        // Comprobar si hay al menos 1 mano agarrada a la pared (aparte de si misma).
        // Si no es el caso, no se permitirá cogerlo.
        if (!_player.IsBodyPartGrabbable(this)) return;
        
        DropHold();
        base.OnMouseDown();
    }

    protected override void OnMouseUp()
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
            Debug.Log("Hold " + holdScr.name + " in range of" + this.name);
            holdInRange = holdScr;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Hold holdScr = other.GetComponent<Hold>();
        if (holdScr != null && holdScr.gameObject == other.gameObject)
        {
            holdInRange = null;
            Debug.Log("Hold " + holdScr.name + " left range of" + this.name);
        }
    }
}
