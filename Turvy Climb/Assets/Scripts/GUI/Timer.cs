using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;

    private float currentTime;

    void OnEnable()
    {
        EventManager.LevelTimePassed += UpdateTimer;
    }

    void OnDisable()
    {
        EventManager.LevelTimePassed += UpdateTimer;
    }

    public void UpdateTimer(float seconds)
    {
        currentTime = seconds;
        timerText.text = Utilities.ConvertToMinutesTimerFormat(currentTime);
    }
}
