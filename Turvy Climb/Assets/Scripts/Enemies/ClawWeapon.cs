using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClawWeapon : EnemyWeapon
{
    public UnityEvent<DraggableBodyPart> dropHand;

    protected new void Awake()
    {
        base.Awake();

        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null)
        {
            dropHand.AddListener(player.DropBodyPart);
        }
    }

    public new void OnTriggerEnter2D(Collider2D other)
    {
        DraggableHand hand = other.GetComponent<DraggableHand>();
        if (hand != null)
        {
            hand.DropHold();
            dropHand.Invoke(hand);
        }

        base.OnTriggerEnter2D(other);
    }
}
