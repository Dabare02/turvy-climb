using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class SpecificAttackHandler : MonoBehaviour
{
    [SerializeField] protected CircleCollider2D hitDetector;

    [NonSerialized] public PlayerAttackTypeSO attackData;

    public virtual bool attackMode
    {
        get
        {
            return hitDetector.enabled;
        }
        set
        {
            hitDetector.enabled = value;
        }
    }

    void Start()
    {
        hitDetector.radius = attackData.range;
        hitDetector.enabled = false;
    }
}
