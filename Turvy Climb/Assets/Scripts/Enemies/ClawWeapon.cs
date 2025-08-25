using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClawWeapon : EnemyWeapon
{
    public new void OnTriggerEnter2D(Collider2D other)
    {
        DraggableHand hand = other.GetComponent<DraggableHand>();
        if (hand != null)
        {
            hand.DropHold();
            EventManager.OnEnemyForcesDropBodyPart(hand);
        }

        base.OnTriggerEnter2D(other);
    }
}
