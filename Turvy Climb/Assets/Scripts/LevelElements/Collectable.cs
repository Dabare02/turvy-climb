using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// IMPORTANTE
// Si se trata de un rábano, no cambiarlo de su posición en la jerarquía ni entre sus hermanos.
// Esto es porque dicho orden se utiliza para comprobar la cantidad de rábanos recogidos.
// Si es necesario hacerlo, considera cambiar la forma en la que se ordenan los rábanos en LevelManager.

public class Collectable : MonoBehaviour
{
    public StaminaCostSO stCost;
    public AudioClip collectedSound;

    void OnTriggerEnter2D(Collider2D other)
    {
        DraggableTorso torso = other.GetComponent<DraggableTorso>();
        if (torso != null)
        {
            // Calcular cambio de aguante.
            StaminaManager stManager = FindObjectOfType<StaminaManager>();
            if (stManager != null)
            {
                // Comprobar si se aumenta la cantidad máxima de aguante.
                if (stCost.isMaxStaminaIncreased)
                {
                    stManager.IncreaseMaxStamina(stCost.maxStaminaIncreaseAmount);

                    // No hace falta avisar de que se ha recoogido el rábano. Si se destruye un GameObject, Unity actua como si todas las referencias a dicho GameObject
                    // fueran null. Se puede usar este hecho para comprobar en LevelManager la cantidad de rábanos recogidos.
                    //LevelManager lvlManager = FindObjectOfType<LevelManager>();
                    //if (lvlManager != null && this.CompareTag("Radish"))
                    //{
                    //    lvlManager.RadishCollected(this);
                    //}
                }

                // Comprobar si se regenera o se drena aguante.
                if (stCost.isStaminaRegened)
                {
                    stManager.IncreaseCurrentStamina(stCost.staminaCost);
                }
                else
                {
                    stManager.DecreaseCurrentStamina(stCost.staminaCost);
                }

                GeneralManager.Instance.audioManager.PlaySound(collectedSound);
                Destroy(gameObject);
            }
        }
    }
}
