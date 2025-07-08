using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : Enemy
{
    protected override void Stun(bool isLargeStun)
    {
        if (isLargeStun) Debug.Log(name + " was stunned for a long time!");
        else Debug.Log(name + " was briefly stunned.");
    }
}
