using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StaminaManager : MonoBehaviour
{
    private float stamina;
    [SerializeField] private float maxStamina;
    [SerializeField] private float initialStamina;

    // Corutina para ir cambiando la cantidad de aguante gradualmente.
    private List<Tuple<MoveEnum, Coroutine>> continuousStChange;

    // Evento disparado por la clase para indicar el cambio de aguante.
    public UnityEvent OnStaminaChange;
    // Evento disparado por la clase para indicar que la cantidad de aguante es 0.
    public UnityEvent OnStaminaDeplete;

    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            OnStaminaDeplete.AddListener(player.GetComponent<Player>().OutOfStamina);
        }

        stamina = initialStamina;

        // Inicializar lista de corutinas.
        continuousStChange = new List<Tuple<MoveEnum, Coroutine>>();
        foreach (MoveEnum m in Enum.GetValues(typeof(MoveEnum)))
        {
            continuousStChange.Add(new Tuple<MoveEnum, Coroutine>(m, null));
        }
    }

    void Update()
    {
        // DEBUG
        if (Input.GetKeyDown(KeyCode.DownArrow)) DecreaseCurrentStamina(10);
        if (Input.GetKeyDown(KeyCode.UpArrow)) IncreaseCurrentStamina(10);
        // DEBUG
    }

    public void DecreaseCurrentStamina(float amount)
    {
        stamina -= amount;
        if (stamina <= 0)
        {
            stamina = 0;
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

    public void StartContinuousStaminaDrain(MoveEnum move, float amount, float delay)
    {
        StopContinuousStaminaChange(move);

        int index = continuousStChange.FindIndex(x => x.Item1 == move);
        continuousStChange[index] = new Tuple<MoveEnum, Coroutine>(move, StartCoroutine(ContinuousStaminaChange(amount, delay)));
        /*
        Debug.LogWarning("Stamina coroutine " + index + " is null? ");
        Debug.LogWarning(continuousStChange[index].Item2 == null);
        */
    }

    public void StartContinuousStaminaRegen(MoveEnum move, float amount, float delay)
    {
        StopContinuousStaminaChange(move);

        int index = continuousStChange.FindIndex(x => x.Item1 == move);
        continuousStChange[index] = new Tuple<MoveEnum, Coroutine>(move, StartCoroutine(ContinuousStaminaChange(amount, delay, true)));
        /*
        Debug.LogWarning("Stamina coroutine " + index + " is null? ");
        Debug.LogWarning(continuousStChange[index].Item2 == null);
        */
    }

    public void StopContinuousStaminaChange(MoveEnum move)
    {
        int index = continuousStChange.FindIndex(x => x.Item1 == move);
        if (continuousStChange[index].Item2 != null)
        {
            StopCoroutine(continuousStChange[index].Item2);
            continuousStChange[index] = new Tuple<MoveEnum, Coroutine>(move, null);
        }
        /*
        Debug.LogWarning("Stamina coroutine " + index + " is null? ");
        Debug.LogWarning(continuousStChange[index].Item2 == null);
        */
    }

    public IEnumerator ContinuousStaminaChange(float amount, float delay, bool isRegen = false)
    {
        while (stamina > 0 && stamina <= maxStamina)
        {
            if (isRegen)
            {
                IncreaseCurrentStamina(amount);
            }
            else
            {
                DecreaseCurrentStamina(amount);
            }

            yield return new WaitForSeconds(delay);
        }
    }

    private void NotifyStaminaChange()
    {
        Debug.Log("Stamina changed to " + stamina);
        OnStaminaChange.Invoke();
        if (stamina <= 0) OnStaminaDeplete.Invoke();
    }
}