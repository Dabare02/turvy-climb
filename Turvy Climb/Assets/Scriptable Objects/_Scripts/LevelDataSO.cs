using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData")]
public class LevelDataSO : ScriptableObject
{
    public int number;
    public string title;
    public Sprite preview;

    // User data
    [NonSerialized] public float totalPlayedTime;
    [NonSerialized] public float recordTime;
    [NonSerialized] public float progress;
    [NonSerialized] public bool[] radishesCollected;
    [NonSerialized] public bool[] stars;
}
