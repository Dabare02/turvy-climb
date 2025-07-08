using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    public EnemyDataSO enemyData;
    [SerializeField] protected EnemyWeapon weapon;
    [NonSerialized] public EnemyState state;
    protected int hitPoints;
    public bool isDead
    {
        get
        {
            return state == EnemyState.DEAD;
        }
    }
    public bool isStunned
    {
        get
        {
            return state == EnemyState.STUNNED;
        }
    }

    protected Animator _anim;

    protected Coroutine stunCoroutine;

    void Awake()
    {
        _anim = GetComponent<Animator>();
        state = EnemyState.STANDBY;

        weapon.SetupWeapon(this, _anim, enemyData.attackType);
    }

    void Start()
    {
        ResetHealth();
    }

    void Update()
    {
        if (isDead) return;

        if (enemyData.attackType.isWeaponAlwaysReady)
        {
            weapon.ReadyWeapon();
        }
    }

    public void ResetHealth()
    {
        hitPoints = enemyData.initialHitPoints;
    }

    protected void Attack()
    {
        weapon.Attack();
    }
    
    public void TakeDamage(int damage, MoveEnum attackType = MoveEnum.NONE)
    {
        if (!enemyData.inmuneToDamage)
        {
            hitPoints -= damage;
            Debug.Log("Enemy " + this.name + " has taken " + damage + " points of damage (" + hitPoints + " remaining).");

            _anim.Play("damaged");

            if (hitPoints <= 0)
            {
                hitPoints = 0;
                Die();
                return;
            }
        }

        if (!enemyData.inmuneToStun)
        {
            switch (attackType)
            {
                case MoveEnum.NONE:
                    break;
                case MoveEnum.PUNCH:
                    Stun(false);
                    break;
                case MoveEnum.SLINGSHOT:
                    Stun(true);
                    break;
            }
        }
    }
    
    protected void Stun(bool isLargeStun)
    {
        stunCoroutine = StartCoroutine(StunCoroutine(isLargeStun));
    }

    private IEnumerator StunCoroutine(bool isLargeStun)
    {
        state = EnemyState.STUNNED;
        weapon.ResetWeapon();

        float stunDuration = isLargeStun ? enemyData.largeStunDuration : enemyData.regularStunDuration;
        _anim.SetBool("Stunned", true);

        yield return new WaitForSeconds(stunDuration);

        _anim.SetBool("Stunned", false);

        state = EnemyState.STANDBY;
    }

    protected void Die()
    {
        state = EnemyState.DEAD;
        weapon.ResetWeapon();
        Debug.Log("Enemy " + this.name + " died!");
        Destroy(gameObject);
    }
}
