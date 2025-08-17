using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAttack", menuName = "ScriptableObjects/Player/PlayerAttack")]
public class PlayerAttackTypeSO : ScriptableObject
{
    public int damage;
    public float range;
    [Tooltip("Rango fuera del que se tiene que arrastrar la parte del cuerpo para que " +
        "cuente como si se estuviera iniciando el ataque.")]
    public float rangeForPerformingAttack;
    public float launchForce;
    public float duration;
    [Tooltip("Cantidad de tiempo adicional que el ataque se mnantiene en un estado en el " +
        "hace daño después de que se termine el período de ataque.")]
    public float extraAttackHitTime;
    public StaminaCostSO staminaData;
    public AudioClip sound;
}
