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
    private bool _staminaChangeLocked = true;

    // Evento disparado por la clase para indicar el cambio de aguante.
    public UnityEvent<float> staminaChangeEvent;
    // Evento disparado por la clase para indicar que la cantidad de aguante es 0.
    public UnityEvent staminaDepleteEvent;

    void Awake()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            staminaDepleteEvent.AddListener(player.OutOfStamina);
        }

        ResetStamina();

        // Inicializar lista de corutinas.
        continuousStChange = new List<Tuple<MoveEnum, Coroutine>>();
        foreach (MoveEnum m in Enum.GetValues(typeof(MoveEnum)))
        {
            continuousStChange.Add(new Tuple<MoveEnum, Coroutine>(m, null));
        }
    }

    void Update()
    {
        // // DEBUG
        // if (Input.GetKeyDown(KeyCode.DownArrow)) DecreaseCurrentStamina(10);
        // if (Input.GetKeyDown(KeyCode.UpArrow)) IncreaseCurrentStamina(10);
        // // DEBUG
    }

    public void ResetStamina()
    {
        stamina = initialStamina;
    }

    public void LockStaminaChange(bool cond)
    {
        _staminaChangeLocked = cond;
        if (cond) StopAllContinuousStaminaChange();
    }

    public void DecreaseCurrentStamina(float amount)
    {
        if (_staminaChangeLocked) return;
        
        stamina -= amount;
        if (stamina <= 0)
        {
            stamina = 0;
        }

        NotifyStaminaChange();
    }

    public void IncreaseCurrentStamina(float amount)
    {
        if (_staminaChangeLocked) return;

        stamina += amount;
        if (stamina >= maxStamina)
        {
            stamina = maxStamina;
        }

        NotifyStaminaChange();
    }

    public void IncreaseMaxStamina(float amount)
    {
        if (_staminaChangeLocked) return;

        maxStamina += amount;
        stamina = maxStamina;
        NotifyStaminaChange();
    }

    public void StartContinuousStaminaDrain(MoveEnum move, float amount, float delay)
    {
        // Se detiene la corutina para el movimiento que se estaba realizando anteriormente (si lo había).
        StopContinuousStaminaChange(move);

        // Se inicia una nueva corutina para el movimiento.
        int index = continuousStChange.FindIndex(x => x.Item1 == move);
        continuousStChange[index] = new Tuple<MoveEnum, Coroutine>(move, StartCoroutine(ContinuousStaminaChange(amount, delay)));

        // Debug.Log("Started draining stamina for " + move);
    }

    public void StartContinuousStaminaRegen(MoveEnum move, float amount, float delay)
    {
        // Se detiene la corutina para el movimiento que se estaba realizando anteriormente (si lo había).
        StopContinuousStaminaChange(move);

        // Se inicia una nueva corutina para el movimiento.
        int index = continuousStChange.FindIndex(x => x.Item1 == move);
        continuousStChange[index] = new Tuple<MoveEnum, Coroutine>(move, StartCoroutine(ContinuousStaminaChange(amount, delay, true)));
    }

    public void StopContinuousStaminaChange(MoveEnum move)
    {
        int index = continuousStChange.FindIndex(x => x.Item1 == move);
        if (continuousStChange[index].Item2 != null)
        {   // Si no hay corutina para el movimiento, no se intenta detener.
            StopCoroutine(continuousStChange[index].Item2);
            continuousStChange[index] = new Tuple<MoveEnum, Coroutine>(move, null);

            // Debug.Log("Stopped draining stamina for " + move);
        }
    }

    public void StopAllContinuousStaminaChange()
    {
        foreach (MoveEnum m in Enum.GetValues(typeof(MoveEnum)))
        {
            int index = continuousStChange.FindIndex(x => x.Item1 == m);
            if (continuousStChange[index].Item2 != null)
            {   // Si no hay corutina para el movimiento, no se intenta detener.
                StopCoroutine(continuousStChange[index].Item2);
                continuousStChange[index] = new Tuple<MoveEnum, Coroutine>(m, null);
            }
        }
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
        // Debug.Log("Stamina changed to " + stamina);
        staminaChangeEvent.Invoke(stamina);
        if (stamina <= 0) staminaDepleteEvent.Invoke();
    }
}