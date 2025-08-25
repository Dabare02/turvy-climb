using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text playedTimeTMP;
    [SerializeField] private TMP_Text radishesTMP;

    public void SetStats(float totalPlayedTime, int totalRadishes) {
        playedTimeTMP.text = Utilities.ConvertToMinutesTimerFormat(totalPlayedTime);
        radishesTMP.text = totalPlayedTime.ToString();
    }
}