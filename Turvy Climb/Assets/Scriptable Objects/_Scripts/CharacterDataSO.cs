using UnityEngine;

[CreateAssetMenu(fileName = "BaseCharacterData", menuName = "ScriptableObjects/CharacterData/BaseCharacterData", order = 2)]
public class CharacterDataSO : ScriptableObject
{
    public int maxHitPoints;
    public int initialHitPoints;
}
