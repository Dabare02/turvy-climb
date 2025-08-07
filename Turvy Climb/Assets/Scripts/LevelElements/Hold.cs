using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hold : MonoBehaviour
{
    [NonSerialized] public bool gripped;
    public bool firstGrip
    {
        get; private set;
    }

    public void FirstGrip()
    {
        firstGrip = true;
    }
}
