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
    private Animator _anim;

    public override bool attackMode
    {
        get => base.attackMode;
        set
        {
            //if (value) _anim.SetTrigger("CloseHand");
            //else _anim.SetTrigger("OpenHand");

            base.attackMode = value;
        }
    }

    void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (!other.isTrigger && enemy != null && attackMode)
        {
            EventManager.OnPunchSucceeded();

            enemy.TakeDamage(attackData.damage, MoveEnum.Punch);
        }
    }
}
