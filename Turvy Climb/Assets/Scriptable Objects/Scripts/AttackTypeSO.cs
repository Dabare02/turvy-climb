using UnityEngine;

[CreateAssetMenu(fileName = "AttackType", menuName = "ScriptableObjects/AttackType", order = 5)]
public class AttackTypeSO : ScriptableObject
{
    public int damage;
    public float range;
    public float finishDuration;
    public float delay;
    public AudioClip sound;
}
