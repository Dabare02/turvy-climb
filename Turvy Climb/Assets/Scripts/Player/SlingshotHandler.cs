using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SlingshotHandler : SpecificAttackHandler
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (!other.isTrigger && enemy != null && attackMode)
        {
            EventManager.OnSlingshotSucceeded();

            enemy.TakeDamage(attackData.damage, MoveEnum.Slingshot);
        }
    }
}
