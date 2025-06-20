using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableHand : DraggableBodyPart
{
    private GameObject grippedHold;

    protected new void OnMouseDown()
    {
        _body.constraints = RigidbodyConstraints2D.FreezeRotation;
        base.OnMouseDown();
    }

    protected new void OnMouseUp()
    {
        _body.constraints = RigidbodyConstraints2D.FreezeAll;
        base.OnMouseUp();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hold"))
        {
            grippedHold = other.gameObject;
            Debug.Log("Grabbed hold");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Hold") && (grippedHold == other.gameObject))
        {
            grippedHold = null;
            Debug.Log("Stopped holding");
        }
    }
}
