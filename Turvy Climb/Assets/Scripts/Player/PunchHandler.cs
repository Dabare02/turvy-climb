using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Se encarga específicamente de detectar enemigos cuando se hace un puñetazo y
// determinar que hacer.
public class PunchHandler : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            Debug.Log("Enemy " + enemy.name + " detected!");
        }
    }
}
