using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private AudioClip mainMenuTheme;

    void Start() {
        if (mainMenuTheme != null) GeneralManager.Instance.audioManager.PlayMusic(mainMenuTheme);
    }

    public void StartGame() {
        GeneralManager.Instance.GoToLevelSelect();
    }

    public void Credits() {
        Debug.LogWarning("Credits not implemented.");
    }

    public void CreditsClose() {
        Debug.LogWarning("Credits not implemented.");
    }

    public void QuitGame() {
        GeneralManager.Instance.Quit();
    }
}
