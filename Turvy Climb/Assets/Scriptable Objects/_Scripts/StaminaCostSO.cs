using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StaminaCost", menuName = "ScriptableObjects/Player/StaminaCost")]
public class StaminaCostSO : ScriptableObject
{
    public float staminaCost;
    [Tooltip("Indica si el costo de aguante se va drenando (o regenerando) a lo largo de la " +
        "duración del evento causante o si es una cantidad de aguante que se drena una sola vez " +
        "ocurre el evento causante.")]
    public bool isStaminaCostImmediate;
    [Tooltip("Cada cuantos segundos se drena (o regenera) la cantidad de aguante indicada. " +
        "Solo se tendrá en cuenta si isStaminaCostImmediate es false.")]
    public float staminaChangeSpeed;
    [Tooltip("Indica si el cambio de cantidad de aguante ocurre al principio o al final " +
        "del evento causante. Solo se tendrá en cuenta si isStaminaCostImmediate es true.")]
    public bool isStaminaChangeAtStart;
    [Tooltip("Indica si la cantidad de aguante se drena o se regenera.")]
    public bool isStaminaRegened;
    [Tooltip("Indica si se incrementa la cantidad máxima de aguante.")]
    public bool isMaxStaminaIncreased;
    [Tooltip("Cantidad de aguante máximo a aumentar. Solo se tiene en cuenta si isMaxStaminaIncreased es true.")]
    public float maxStaminaIncreaseAmount;
}
