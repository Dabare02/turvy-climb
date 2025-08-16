using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Slider progressBar;

    public void SetValue(float amount)
    {
        progressBar.value = amount;
    }
}
