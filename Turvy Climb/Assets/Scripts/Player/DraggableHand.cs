using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

// Representa una mano que se puede mover. Tambien se encarga de la detección de colisiones.
public class DraggableHand : DraggableBodyPart
{
    public List<Hold> holdsInRange;

    public Hold grippedHold
    {
        get; private set;
    }

    public bool isGripped
    {
        get
        {
            return grippedHold != null && grippedHold.gripped;
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

        holdsInRange = new List<Hold>();
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
        if (gripEnabled && grippedHold == null
            && hold != null && !hold.gripped)
        {
            // Indicar al saliente que hay una extremidad sujeta a este.
            hold.gripped = true;

            // Establecer nuevo saliente como grippedHold.
            if (grippedHold == null) grippedHold = hold;

            // Mover mano y congelar su RigidBody.
            transform.position = new Vector3(hold.transform.position.x, hold.transform.position.y, transform.position.z);
            _body.constraints = RigidbodyConstraints2D.FreezeAll;

            // Indicar a Player que hay un saliente más al que está agarrado.
            _player.IncreaseGrippedHolds(1);
            
            Debug.Log(this.name + " is gripping hold " + grippedHold.name);
        }
    }

    public void DropHold()
    {
        if (grippedHold != null && grippedHold.gripped)
        {
            Debug.Log(this.name + " is dropping hold " + grippedHold.name);
            // Descongelar RigidBody.
            _body.constraints = RigidbodyConstraints2D.FreezeRotation;
            _body.AddForce(Vector2.zero);   // Para forzar update. No recomendado, quizás usar corutina con "yield return new WaitForFixedUpdate()".

            // Indicar al saliente que ya no hay extremiades sujetas a este.
            grippedHold.gripped = false;

            // Desasignar saliente.
            grippedHold = null;

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

        // Encontrar saliente dentro del rango más cercano a la mano o pie.
        Hold holdToGrip = ClosestHold();

        // Agarrar saliente.
        GripHold(holdToGrip);
        base.OnMouseUp();
    }

    /// <summary>
    /// Encuentra el saliente dentro del rango de la mano o pie que esté más cercano a este.
    /// </summary>
    /// <returns>Devuelve el saliente, null si no hay ningún saliente dentro del rango.</returns>
    public Hold ClosestHold()
    {
        Hold closestHold = null;
        if (holdsInRange.Count > 0) {
            List<Transform> transforms = holdsInRange.Select(x => x.transform).ToList();
            Transform closestToHand = Utilities.ClosestTransformToTarget(this.transform, transforms);
            closestHold = holdsInRange.Find(x => x.transform == closestToHand);
        }

        return closestHold;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Hold holdScr = other.GetComponent<Hold>();
        if (holdScr != null && !holdScr.gripped)
        {
            holdsInRange.Add(holdScr);
            Debug.Log("Hold " + holdScr.name + " in range of " + this.name + " (Current holds in range: " + holdsInRange.ToSeparatedString(", "));
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Hold holdScr = other.GetComponent<Hold>();
        if (holdScr != null && holdScr.gameObject == other.gameObject)
        {
            holdsInRange.Remove(holdScr);
            Debug.Log("Hold " + holdScr.name + " left range of " + this.name + " (Current holds in range: " + holdsInRange.ToSeparatedString(", "));
        }
    }
}
