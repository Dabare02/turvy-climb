using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class SpecificAttackHandler : MonoBehaviour
{
    [SerializeField] protected CircleCollider2D hitDetector;

    [NonSerialized] public PlayerAttackTypeSO attackData;

    public virtual bool attackMode
    {
        get
        {
            return hitDetector.enabled;
        }
        set
        {
            hitDetector.enabled = value;
            Debug.Log("AttackMode active? " + value);
        }
    }

    void Start()
    {
        hitDetector.radius = attackData.range;
        hitDetector.enabled = false;
    }

    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     Enemy enemy = other.GetComponent<Enemy>();
    //     if (!other.isTrigger && enemy != null && attackMode)
    //     {
    //         // Debug.Log("Enemy " + enemy.name + " detected!");
    //         // if (_handCollider != null) _handCollider.enabled = true;
    //         attackSuccessEvent.Invoke();
    //         attackMode = false;

    //         enemy.TakeDamage(attackData.damage, MoveEnum.PUNCH);
    //     }
    // }
}
