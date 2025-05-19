using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableHand : DraggableBodyPart
{
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
}
