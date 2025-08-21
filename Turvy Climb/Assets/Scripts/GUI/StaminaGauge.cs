using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StaminaGauge : MonoBehaviour
{
    [SerializeField] private Slider staminaBar;
    [SerializeField] private Image staminaFill;
    [SerializeField] private TMP_Text staminaNumber;
    [SerializeField] private Gradient staminaFillGradient;

    void OnEnable()
    {
        EventManager.StaminaAmountChanged += SetStamina;
        EventManager.MaxStaminaAmountChanged += SetMaxStamina;
    }

    void OnDisable()
    {
        EventManager.StaminaAmountChanged -= SetStamina;
        EventManager.MaxStaminaAmountChanged -= SetMaxStamina;
    }

    public void SetStamina(float amount)
    {
        if (amount < 0 || amount > staminaBar.maxValue)
        {
            Debug.LogWarning("The stamina amount indicated is not within an acceptable range. "
                + "Setting to max value instead.");
        }
        staminaBar.value = amount;
        staminaNumber.text = staminaBar.value.ToString("F2");

        staminaFill.color = staminaFillGradient.Evaluate(staminaBar.normalizedValue);
    }

    public void SetMaxStamina(float amount)
    {
        staminaBar.maxValue = amount;
    }
}
