using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Slider progressBar;

    void OnEnable()
    {
        EventManager.LevelProgressChanged += SetValue;
    }

    void OnDisable()
    {
        EventManager.LevelProgressChanged -= SetValue;
    }

    public void SetValue(float amount)
    {
        progressBar.value = amount;
    }
}
