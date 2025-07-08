using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public abstract class Enemy : MonoBehaviour
{
    public EnemyDataSO enemyData;
    [SerializeField] private EnemyWeapon weapon;
    private int hitPoints;
    public bool isDead { get; private set; }
    public bool isStunned { get; private set; }
    public bool isAttackReady { get; private set; }

    private Animator _anim;

    void Awake()
    {
        _anim = GetComponent<Animator>();

        weapon.attackData = enemyData.attackType;
        weapon.parentAnim = _anim;
    }

    void Start()
    {
        ResetHealth();
    }

    public void ResetHealth()
    {
        hitPoints = enemyData.initialHitPoints;
    }

    public void TakeDamage(int damage, MoveEnum attackType = MoveEnum.None)
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
                case MoveEnum.None:
                    break;
                case MoveEnum.Punch:
                    Stun(false);
                    break;
                case MoveEnum.Slingshot:
                    Stun(true);
                    break;
            }
        }
    }

    protected void Die()
    {
        isDead = true;
        Debug.Log("Enemy " + this.name + " died!");
        Destroy(gameObject);
    }

    protected void Attack()
    {
        weapon.Attack();
    }
    
    protected abstract void Stun(bool isLargeStun);
}
