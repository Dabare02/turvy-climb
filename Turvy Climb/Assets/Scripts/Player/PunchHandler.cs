using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

// Se encarga específicamente de detectar enemigos cuando se hace un puñetazo y
// determinar que hacer.
public class PunchHandler : MonoBehaviour
{
    [SerializeField] private CircleCollider2D hitDetector;

    private CircleCollider2D _handCollider;
    [NonSerialized] public PlayerAttackTypeSO punchAttack;

    // Events
    public UnityEvent onAttackSuccess;

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

    void Start()
    {
        hitDetector.radius = punchAttack.attackRange;
        hitDetector.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && attackMode)
        {
            //Debug.Log("Enemy " + enemy.name + " detected!");
            //if (_handCollider != null) _handCollider.enabled = true;
            onAttackSuccess.Invoke();
            attackMode = false;

            enemy.TakeDamage(punchAttack.attackDamage);
        }
    }
}
