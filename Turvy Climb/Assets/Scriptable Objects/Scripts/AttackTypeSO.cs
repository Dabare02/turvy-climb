using UnityEngine;

[CreateAssetMenu(fileName = "AttackType", menuName = "ScriptableObjects/AttackType", order = 5)]
public class AttackTypeSO : ScriptableObject
{
    public int attackDamage;
    public float attackRange;
    public float attackDuration;
    public AudioClip attackSound;
}
