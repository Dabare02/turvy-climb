using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData/EnemyData")]
public class EnemyDataSO : CharacterDataSO
{
    public float speed;
    public float regularStunDuration;
    public float largeStunDuration;
    public bool inmuneToDamage;
    public bool inmuneToRegularStun;
    public bool inmuneToLargeStun;
    public EnemyAttackTypeSO attackType;
}
