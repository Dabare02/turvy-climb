using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableBody : DraggableBodyPart
{
    [Tooltip("Puntos de ancla para cada brazo. Orden: [brazoIzquierdo, brazoDerecho, piernaIzquierda, piernaDerecha]")]
    [SerializeField] private Transform[] anchorPoints;

    void Start()
    {
        base.Start();

        SpringJoint2D[] springs = GetComponents<SpringJoint2D>();
        for (int i = 0; i < springs.Length; i++) {
            // Podríamos calcular distance solo 1 vez, pero hacemos array por si en un futuro los
            // anchorPoints tienen posiciones irregulares.
            float distance = Vector2.Distance(anchorPoints[i].position, Vector2.zero);
            springs[i].distance = distance + 0.5f;
        }
    }
}
