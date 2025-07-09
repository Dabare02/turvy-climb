using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAttack", menuName = "ScriptableObjects/EnemyData/EnemyAttack")]
public class EnemyAttackTypeSO : ScriptableObject
{
    public float damage;
    public float range;
    public float finishDuration;
    public float cooldown;
    [Tooltip("Indica si el arma esta siempre activa o si se deberá activar")]
    public bool isWeaponAlwaysReady;
    public AudioClip sound;
}
