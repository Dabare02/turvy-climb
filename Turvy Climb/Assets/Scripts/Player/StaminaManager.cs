using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StaminaManager : MonoBehaviour
{
    private float stamina;
    [Tooltip("Si el valor es <0, su valor se cambiará para igualar el aguante máximo.")]
    [SerializeField] private float initialStamina = -1;
    // Corutina para ir cambiando la cantidad de aguante gradualmente.
    private List<Tuple<MoveEnum, Coroutine>> continuousStChange;
    private bool _staminaChangeLocked = true;

    // Evento disparado por la clase para indicar el cambio de aguante.
    public UnityEvent<float> onSTChange;
    // Evento disparado para indicar que la cantidad de aguante máxima ha aumentado.
    public UnityEvent<float> onMaxSTChange;
    // Evento disparado por la clase para indicar que la cantidad de aguante es 0.
    public UnityEvent staminaDepleteEvent;

    void Awake()
    {
        // Inicializar lista de corutinas.
        continuousStChange = new List<Tuple<MoveEnum, Coroutine>>();
        foreach (MoveEnum m in Enum.GetValues(typeof(MoveEnum)))
        {
            continuousStChange.Add(new Tuple<MoveEnum, Coroutine>(m, null));
        }
    }

    void OnEnable()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            staminaDepleteEvent.AddListener(player.OutOfStamina);
        }

        StaminaGauge stGauge = FindObjectOfType<StaminaGauge>();
        if (stGauge != null)
        {
            onSTChange.AddListener(stGauge.SetStamina);
            onMaxSTChange.AddListener(stGauge.SetMaxStamina);
        }
    }

    void Start()
    {
        if (initialStamina <= 0) initialStamina = GeneralManager.Instance.maxPlayerStamina;
        ResetStamina();
        NotifyMaxStaminaChange();
    }

    void Update()
    {
        // // DEBUG
        // if (Input.GetKeyDown(KeyCode.DownArrow)) DecreaseCurrentStamina(10);
        // if (Input.GetKeyDown(KeyCode.UpArrow)) IncreaseCurrentStamina(10);
        // // DEBUG
    }

    void OnDisable()
    {
        onSTChange.RemoveAllListeners();
        onMaxSTChange.RemoveAllListeners();
        staminaDepleteEvent.RemoveAllListeners();
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
        if (stamina >= GeneralManager.Instance.maxPlayerStamina)
        {
            stamina = GeneralManager.Instance.maxPlayerStamina;
        }

        NotifyStaminaChange();
    }

    public void IncreaseMaxStamina(float amount)
    {
        if (_staminaChangeLocked) return;

        GeneralManager.Instance.maxPlayerStamina += amount;
        stamina = GeneralManager.Instance.maxPlayerStamina;
        NotifyMaxStaminaChange();
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

    private IEnumerator ContinuousStaminaChange(float amount, float delay, bool isRegen = false)
    {
        while (stamina > 0 && stamina <= GeneralManager.Instance.maxPlayerStamina)
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

    public void DepleteStamina()
    {
        DecreaseCurrentStamina(10000f);
    }

    private void NotifyMaxStaminaChange()
    {
        Debug.Log("Max stamina increased to " + GeneralManager.Instance.maxPlayerStamina);
        onMaxSTChange.Invoke(GeneralManager.Instance.maxPlayerStamina);
        NotifyStaminaChange();
    }

    private void NotifyStaminaChange()
    {
        Debug.Log("Stamina changed to " + stamina);
        onSTChange.Invoke(stamina);
        if (stamina <= 0) staminaDepleteEvent.Invoke();
    }
}