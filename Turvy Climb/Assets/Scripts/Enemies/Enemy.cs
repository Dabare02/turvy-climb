using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public abstract class Enemy : MonoBehaviour
{
    public EnemyDataSO enemyData;
    private int hitPoints;
    public bool isDead { get; private set; }
    public bool isStunned { get; private set; }

    public UnityEvent<float> playerDamaged;

    void Awake()
    {
        GameObject staminaMngObj = GameObject.FindGameObjectWithTag("LevelManager");
        if (staminaMngObj != null)
        {
            playerDamaged.AddListener(staminaMngObj.GetComponent<StaminaManager>().DecreaseCurrentStamina);
        }
    }

    void Start()
    {
        ResetHealth();
    }

    void OnDisable()
    {
        playerDamaged.RemoveAllListeners();
    }

    public void ResetHealth()
    {
        hitPoints = enemyData.initialHitPoints;
    }

    public void TakeDamage(int damage, MoveEnum attackType = MoveEnum.None)
    {
        hitPoints -= damage;
        Debug.Log("Enemy " + this.name + " has taken " + damage + " points of damage (" + hitPoints + " remaining).");
        if (hitPoints <= 0)
        {
            hitPoints = 0;
            Die();
            return;
        }

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

    protected abstract void Stun(bool isLargeStun);

    protected void DamagePlayer()
    {
        playerDamaged.Invoke(enemyData.attackType.damage);
    }

    protected void Die()
    {
        isDead = true;
        Debug.Log("Enemy " + this.name + " died!");
        Destroy(gameObject);
    }
}
