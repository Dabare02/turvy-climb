using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAttack", menuName = "ScriptableObjects/Player/PlayerAttack", order = 1)]
public class PlayerAttackTypeSO : AttackTypeSO
{
    public float rangeForPerformingAttack;
    public float launchForce;
    public float extraAttackHitTime;
}
