using UnityEngine;

[CreateAssetMenu(fileName = "BaseCharacterData", menuName = "ScriptableObjects/CharacterData/BaseCharacterData")]
public class CharacterDataSO : ScriptableObject
{
    public int maxHitPoints;
    public int initialHitPoints;
}
