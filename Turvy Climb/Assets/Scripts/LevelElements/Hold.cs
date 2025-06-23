using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hold : MonoBehaviour
{
    public bool gripped
    {
        private set; get;
    }

    public void Grip() { gripped = true; }
    public void UnGrip() { gripped = false; }
}
