using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

// Se encarga específicamente de detectar enemigos cuando se hace un puñetazo y
// determinar que hacer.
public class PunchHandler : SpecificAttackHandler
{
    // Events
    public UnityEvent punchSuccessEvent;
    
    public override bool attackMode
    {
        get
        {
            return hitDetector.enabled;
        }
        set
        {
            hitDetector.enabled = value;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (!other.isTrigger && enemy != null && attackMode)
        {
            // Debug.Log("Enemy " + enemy.name + " detected!");
            // if (_handCollider != null) _handCollider.enabled = true;
            punchSuccessEvent.Invoke();
            attackMode = false;

            enemy.TakeDamage(attackData.damage, MoveEnum.Punch);
        }
    }
}
