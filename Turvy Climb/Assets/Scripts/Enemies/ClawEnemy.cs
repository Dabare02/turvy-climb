using System.Collections;
using System.Collections.Generic;
using Cinemachine.Editor;
using UnityEngine;

public class ClawEnemy : Enemy
{
    [Header("Claw parameters")]
    [SerializeField] private Transform weaponParent;
    public CircleCollider2D playerCloseDetector;
    public CircleCollider2D playerImminentDetector;

    private bool _attacking;
    private bool _readying;

    protected new void Update()
    {
        base.Update();

        if (state == EnemyState.STUNNED)
        {
            playerCloseDetector.enabled = false;
            playerImminentDetector.enabled = false;
        }
        else
        {
            playerCloseDetector.enabled = true;
            playerImminentDetector.enabled = true;
        }

        if (_readying)
        {
            weapon.ReadyWeapon();
            //Debug.Log("Readying. " + state);
        }
        if (_attacking)
        {
            weapon.Attack();
            //Debug.Log("Attacking. " + state);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        //Debug.Log("Trigger enter");
        if (playerCloseDetector.IsTouching(other))
        {
            //Debug.Log("Close");
            _readying = true;
        }
        if (playerImminentDetector.IsTouching(other))
        {
            //Debug.Log("Imminent");
            _attacking = true;
        }

        //Debug.Log("Readying? " + _readying + ", attacking? " + _attacking);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        //Debug.Log("Trigger exit");
        if (!playerImminentDetector.IsTouching(other))
        {
            //Debug.Log("Not imminent");
            _attacking = false;
        }
        if (!playerCloseDetector.IsTouching(other))
        {
            //Debug.Log("Not close");
            _readying = false;
            weapon.UnreadyWeapon();
        }

        //Debug.Log("Readying? " + _readying + ", attacking? " + _attacking);
    }
}
