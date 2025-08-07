using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SlingshotHandler : SpecificAttackHandler
{
    // Events
    public UnityEvent slingshotStopEvent;
    
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
            enemy.TakeDamage(attackData.damage, MoveEnum.Slingshot);
        }
    }
}
