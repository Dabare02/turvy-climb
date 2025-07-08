using UnityEngine;

[CreateAssetMenu(fileName = "AttackType", menuName = "ScriptableObjects/AttackType", order = 5)]
public class AttackTypeSO : ScriptableObject
{
    public float damage;
    public float range;
    public float finishDuration;
    public float cooldown;
    [Tooltip("Indica si el arma esta siempre activa o si se deberá activar")]
    public bool isWeaponAlwaysReady;
    public AudioClip sound;
}
