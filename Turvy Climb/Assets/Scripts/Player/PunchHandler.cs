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
    void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (!other.isTrigger && enemy != null && attackMode)
        {
            // Debug.Log("Enemy " + enemy.name + " detected!");
            // if (_handCollider != null) _handCollider.enabled = true;
            EventManager.OnPunchSucceeded();

            enemy.TakeDamage(attackData.damage, MoveEnum.Punch);
        }
    }
}
