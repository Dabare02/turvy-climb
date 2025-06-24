using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableTorso : DraggableBodyPart
{
    [SerializeField] private float springFrequency = 1f;
    [SerializeField] private float springDistance = 1.4f;
    [Range(0, 1)]
    [SerializeField] private float springDampRatio = 0.5f;
    //[Tooltip("Puntos de ancla para cada brazo. Orden: [brazoIzquierdo, brazoDerecho, piernaIzquierda, piernaDerecha]")]
    //[SerializeField] private Transform[] anchorPoints;

    void Awake()
    {
        base.Awake();

        SpringJoint2D[] springs = GetComponents<SpringJoint2D>();
        for (int i = 0; i < springs.Length; i++)
        {
            // ASIGNAR DISTANCIA DE MUELLE
            springs[i].distance = springDistance;

            // ASIGNAR FRECUENCIA DE MUELLE
            springs[i].frequency = springFrequency;

            // ASIGNAR DAMPING RATIO DE MUELLE
            springs[i].dampingRatio = springDampRatio;
        }
    }
}