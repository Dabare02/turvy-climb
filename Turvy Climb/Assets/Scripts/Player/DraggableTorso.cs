using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableTorso : DraggableBodyPart
{
    [SerializeField] private float springFrequency = 2f;
    [SerializeField] private float springDistance = 1.4f;
    //[Tooltip("Puntos de ancla para cada brazo. Orden: [brazoIzquierdo, brazoDerecho, piernaIzquierda, piernaDerecha]")]
    //[SerializeField] private Transform[] anchorPoints;

    void Awake()
    {
        base.Awake();

        SpringJoint2D[] springs = GetComponents<SpringJoint2D>();
        for (int i = 0; i < springs.Length; i++)
        {
            /*
            // CALCULAR DISTANCIA DE SPRINGS.
            // Podríamos calcular distance solo 1 vez, pero hacemos array por si en un futuro los
            // anchorPoints tienen posiciones irregulares.
            float distance = Vector2.Distance(anchorPoints[i].position, Vector2.zero);
            springs[i].distance = distance;
            */

            // ASIGNAR DISTANCIA DE MUELLE
            springs[i].distance = springDistance;

            // ASIGNAR FRECUENCIA DE MUELLE
            springs[i].frequency = springFrequency;
        }
    }
}