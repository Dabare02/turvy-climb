using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CrowEnemy : Enemy
{
    [Header("Movement path")]
    public Transform pointA;
    public Transform pointB;

    private Rigidbody2D _rb;

    private bool _returnToPointA;

    protected new void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        base.Awake();
    }

    protected new void Start()
    {
        base.Start();
        
        ((CircleCollider2D)weapon.hitDetector).radius = enemyData.attackType.range;

        transform.position = pointA.position;
        _returnToPointA = false;
    }

    protected new void Update()
    {
        base.Update();

        Move();
    }

    private void Move()
    {
        // Determinar a que punto ir.
        Transform nextPoint;
        if (_returnToPointA) nextPoint = pointA;
        else nextPoint = pointB;

        // Posición a la que moverse.
        Vector2 newPos = Vector2.MoveTowards(transform.position, nextPoint.position, Time.deltaTime * enemyData.speed);
        
        if (Vector2.Distance(nextPoint.position, transform.position) < 0.1)
        {   // Si ya está en la posición destino, cambiar destino al otro punto.
            GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
            _returnToPointA = !_returnToPointA;
        }
        else
        {   // Si no, seguir moviendose a destino
            _rb.MovePosition(newPos);
        }
    }
}
