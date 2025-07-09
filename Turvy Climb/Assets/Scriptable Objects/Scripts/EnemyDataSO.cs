using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData/EnemyData")]
public class EnemyDataSO : CharacterDataSO
{
    public float speed;
    public float regularStunDuration;
    public float largeStunDuration;
    public bool inmuneToDamage;
    public bool inmuneToStun;
    public EnemyAttackTypeSO attackType;
}
