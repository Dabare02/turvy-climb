using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData")]
public class LevelDataSO : ScriptableObject
{
    public int number;
    public string title;

    // User data
    [NonSerialized] public float totalPlayedTime;
    [NonSerialized] public float recordTime;
    [NonSerialized] public Dictionary<int, bool> radishesCollected;
    [NonSerialized] public Dictionary<int, bool> stars;
}
