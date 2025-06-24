using UnityEngine;

[CreateAssetMenu(fileName = "AttackType", menuName = "ScriptableObjects/AttackType")]
public class AttackTypeSO : ScriptableObject
{
    public float attackDamage;
    public float attackRange;
    public AudioClip attackSound;
}
