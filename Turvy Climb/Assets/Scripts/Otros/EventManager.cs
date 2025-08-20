using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    public static event Action<bool> OnDSAVaueChanged;

    public static void TriggerDontShowAgainValueChanged(bool cond)
    {
        OnDSAVaueChanged?.Invoke(cond);
    }
}
