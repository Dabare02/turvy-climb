using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStaminaManager : MonoBehaviour
{
    private float stamina;
    [SerializeField] private float maxStamina;
    [SerializeField] private float initialStamina;

    private Coroutine continuousStaminaChange;

    public UnityEvent OnStaminaChange;

    public void DecreaseCurrentStamina(float amount)
    {
        stamina -= amount;
        if (stamina <= 0)
        {
            stamina = 0;
            // Drop all holds
        }

        NotifyStaminaChange();
    }

    public void IncreaseCurrentStamina(float amount)
    {
        stamina += amount;
        if (stamina >= maxStamina)
        {
            stamina = maxStamina;
        }

        NotifyStaminaChange();
    }

    public void IncreaseMaxStamina(float amount)
    {
        maxStamina += amount;
        stamina = maxStamina;
        NotifyStaminaChange();
    }

    public void StartContinuousStaminaDrain(float amount, float delay)
    {
        StopContinuousStaminaChange();
        StartCoroutine(ContinuousStaminaChange(amount, delay));
    }

    public void StartContinuousStaminaRegen(float amount, float delay)
    {
        StopContinuousStaminaChange();
        StartCoroutine(ContinuousStaminaChange(amount, delay, true));
    }

    public void StopContinuousStaminaChange()
    {
        if (continuousStaminaChange != null) StopCoroutine(continuousStaminaChange);
    }

    public IEnumerator ContinuousStaminaChange(float amount, float delay, bool isRegen = false)
    {
        float stChangeAmount = amount;
        if (isRegen) stChangeAmount = -stChangeAmount;
        
        while (stamina >= 0 && stamina <= maxStamina)
        {
            stamina -= stChangeAmount;
            NotifyStaminaChange();
            yield return new WaitForSeconds(delay);
        }
    }

    private void NotifyStaminaChange()
    {
        Debug.Log("Stamina changed to " + stamina);
        OnStaminaChange.Invoke();
    }
}