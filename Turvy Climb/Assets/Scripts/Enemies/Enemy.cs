using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyDataSO enemyData;
    private int hitPoints;

    void Start()
    {
        hitPoints = enemyData.initialHitPoints;
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        if (hitPoints <= 0)
        {
            hitPoints = 0;
            Die();
        }
        Debug.Log("Enemy " + this.name + " has taken " + damage + " points of damage (" + hitPoints + " remaining).");
    }

    public void Die()
    {
        Debug.Log("Enemy " + this.name + " died!");
        gameObject.SetActive(false);
    }
}
