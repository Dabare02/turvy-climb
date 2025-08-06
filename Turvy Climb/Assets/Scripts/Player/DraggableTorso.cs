using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableTorso : DraggableBodyPart
{
    [SerializeField] public float springFrequency = 1f;
    [SerializeField] public float springDistance = 1.4f;
    [Range(0, 1)]
    [SerializeField] public float springDampRatio = 0.5f;

    private SpringJoint2D[] _springs;

    protected override void Awake()
    {
        base.Awake();

        _springs = GetComponents<SpringJoint2D>();

        ResetSpringValues();
    }

    protected override void OnMouseDown()
    {
        // Comprobar si el jugador está sujeto a algún saliente.
        // Si no es el caso, no se permitirá cogerlo.
        if (!_player.IsBodyPartGrabbable(this)) return;

        base.OnMouseDown();
    }

    protected override void OnMouseUp()
    {
        // Comprobar si el jugador está sujeto a algún saliente.
        // Si no es el caso, no se permitirá cogerlo.
        if (!_player.IsBodyPartGrabbable(this)) return;

        base.OnMouseUp();
    }

    /// <summary>
    /// Cambia la frecuencia de los SpringJoint2D de cada extremidad. No asignar valor a newFrequency para devolver frecuencia a su valor por defecto.
    /// </summary>
    /// <param name="newFrequency"></param>
    public void ChangeSpringFrequency(float newFrequency)
    {
        float newF = newFrequency;
        if (newFrequency < 0f) newF = springFrequency;

        for (int i = 0; i < _springs.Length; i++)
        {
            _springs[i].frequency = newF;
        }
    }

    /// <summary>
    /// Cambia la distancia de los SpringJoint2D de cada extremidad. No asignar valor a newDistance para devolver distancia a su valor por defecto.
    /// </summary>
    /// <param name="newDistance"></param>
    public void ChangeSpringDistance(float newDistance)
    {
        float newD = newDistance;
        if (newDistance < 0f) newD = springDistance;

        for (int i = 0; i < _springs.Length; i++)
        {
            _springs[i].distance = newD;
        }
    }

    /// <summary>
    /// Cambia el ratio de amortiguación de los SpringJoint2D de cada extremidad. No asignar valor a newDamp para devolver ratio de amortiguación a su valor por defecto.
    /// </summary>
    /// <param name="newDamp"></param>
    public void ChangeSpringDampenRatio(float newDamp)
    {
        float newDR = newDamp;
        if (newDamp < 0f) newDR = springDampRatio;

        if (newDR > 1f)
        {
            Debug.LogError("El ratio de amortguación debe ser un valor en el rango [0,1].");
            return;
        }

        for (int i = 0; i < _springs.Length; i++)
        {
            _springs[i].dampingRatio = newDR;
        }
    }

    public void ResetSpringValues()
    {
        for (int i = 0; i < _springs.Length; i++)
        {
            // ASIGNAR DISTANCIA DE MUELLE
            _springs[i].distance = springDistance;

            // ASIGNAR FRECUENCIA DE MUELLE
            _springs[i].frequency = springFrequency;

            // ASIGNAR DAMPING RATIO DE MUELLE
            _springs[i].dampingRatio = springDampRatio;
        }
    }
}