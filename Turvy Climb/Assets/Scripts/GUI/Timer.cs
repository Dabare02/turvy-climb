using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;

    private LevelManager _lvlManager;
    private float currentTime;

    void Awake()
    {
        _lvlManager = FindObjectOfType<LevelManager>();
    }

    void OnEnable()
    {
        _lvlManager.onSecondPassed.AddListener(UpdateTimer);
    }

    public void UpdateTimer(float seconds)
    {
        currentTime = seconds;
        timerText.text = Utilities.ConvertToMinutesTimerFormat(currentTime);
    }
}
