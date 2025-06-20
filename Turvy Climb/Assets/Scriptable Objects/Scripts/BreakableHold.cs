using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BreakableHold", menuName = "ScriptableObjects/Hold/BreakableHold")]
public class BreakableHold : ScriptableObject
{
    [Tooltip("\"True\" si el saliente se romperá tras estar agarrado a el durante un tiempo (saliente inestable)." + 
        "\"False\" si el saliente se romperá tras agarrarlo una cierta cantidad de veces (saliente frágil).")]
    public bool breaksByTime;
    [Tooltip("Tiempo necesario para que el saliente inestable se desmorone.")]
    public float timeBeforeBreak;
    [Tooltip("Cantidad de veces que hace falta agarrarse al saliente frágil para que se desmorone.")]
    public int attemptsBeforeBreak;
}
