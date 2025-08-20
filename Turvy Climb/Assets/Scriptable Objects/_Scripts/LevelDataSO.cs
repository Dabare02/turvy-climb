using System;
using System.Collections.Generic;
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
    [NonSerialized] public bool dontShowTutorialAgain;
    [NonSerialized] public bool[] stars;
    [NonSerialized] public bool[] radishesCollected;
}
