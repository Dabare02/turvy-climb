using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Se encarga específicamente de detectar enemigos cuando se hace un puñetazo y
// determinar que hacer.
public class PunchHandler : MonoBehaviour
{
    [SerializeField] private CircleCollider2D hitDetector;

    private CircleCollider2D _handCollider;
    [NonSerialized] public PlayerAttackTypeSO punchAttack;

    public bool attackMode
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

    void Awake()
    {
        _handCollider = GetComponent<CircleCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && attackMode)
        {
            //Debug.Log("Enemy " + enemy.name + " detected!");
            _handCollider.enabled = true;

            enemy.TakeDamage(punchAttack.attackDamage);
        }
    }
}
