using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData")]
public class LevelDataSO : ScriptableObject
{
    public int number;
    public string title;
    public int maxRadishes;

    // User data
    [NonSerialized] public float recordTime = 0;
    [NonSerialized] public int collectedRadishes = 0;
    [NonSerialized] public int stars = 0;
}
