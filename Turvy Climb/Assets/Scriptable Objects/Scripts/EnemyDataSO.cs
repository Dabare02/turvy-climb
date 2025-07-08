using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/CharacterData/EnemyData", order = 3)]
public class EnemyDataSO : CharacterDataSO
{
    public float speed;
    public float regularStunDuration;
    public float largeStunDuration;
    public AttackTypeSO attackType;
}
