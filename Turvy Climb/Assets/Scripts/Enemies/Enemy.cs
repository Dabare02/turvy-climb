using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public abstract class Enemy : MonoBehaviour
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

    protected void Awake()
    {
        _anim = GetComponent<Animator>();
        state = EnemyState.STANDBY;

        // Darle a la arma los parámetros necesarios.
        weapon.SetupWeapon(this, _anim, enemyData.attackType);
    }

    protected void Start()
    {
        ResetHealth();
    }

    protected void Update()
    {
        // En el caso de que su arma sea un aura de daño permanente, se tendrá que
        // resetear el arma tanto como se pueda.
        if (enemyData.attackType.isWeaponAlwaysReady && state == EnemyState.STANDBY)
        {
            weapon.ReadyWeapon();
        }

        // DEBUG v
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log(state);
        }
        // DEBUG ^
    }

    public void ResetHealth()
    {
        hitPoints = enemyData.initialHitPoints;
    }

    // Usado si el enemigo tiene una arma que usar (y no un área de daño).
    protected void Attack()
    {
        weapon.Attack();
    }

    public void TakeDamage(int damage, MoveEnum attackType = MoveEnum.NONE)
    {
        bool stunned = false;

        // Determinar la cantidad de daño recibido y morir si procede.
        if (!enemyData.inmuneToDamage)
        {
            hitPoints -= damage;
            Debug.Log("Enemy " + this.name + " has taken " + damage + " points of damage (" + hitPoints + " remaining).");

            if (hitPoints <= 0)
            {
                hitPoints = 0;
                Die();
                return; // Si está muerto, no se quiere comprobar si esta aturdido.
            }
        }

        // Determinar si el enemigo se aturde y aturdir si procede.
        switch (attackType)
        {   // La cantidad de tiempo que estará aturdido depende del enemigo y del tipo de ataque.
            case MoveEnum.PUNCH:
                if (!enemyData.inmuneToRegularStun)
                {
                    stunned = true;
                    Stun(false);
                }

                break;
            case MoveEnum.SLINGSHOT:
                if (!enemyData.inmuneToLargeStun)
                {
                    stunned = true;
                    Stun(true);
                }

                break;
        }

        // Reproducir animación de daño en caso de no ser aturdido.
        if (!stunned) _anim.SetTrigger("Damaged");
    }

    protected void Stun(bool isLargeStun)
    {
        stunCoroutine = StartCoroutine(StunCoroutine(isLargeStun));
    }

    private IEnumerator StunCoroutine(bool isLargeStun)
    {   
        // Inciar
        weapon.ResetWeapon();
        state = EnemyState.STUNNED;
        float stunDuration = isLargeStun ? enemyData.largeStunDuration : enemyData.regularStunDuration;

        _anim.SetBool("Stunned", true);
        yield return new WaitForSeconds(stunDuration);
        _anim.SetBool("Stunned", false);

        state = EnemyState.STANDBY;
    }

    protected void Die()
    {
        weapon.ResetWeapon();
        state = EnemyState.DEAD;

        Debug.Log("Enemy " + this.name + " died!");
        Destroy(gameObject);
    }
}
