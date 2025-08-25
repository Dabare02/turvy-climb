using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialMenu;
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private AudioClip mainMenuTheme;

    void Start()
    {
        if (mainMenuTheme != null) GeneralManager.Instance.audioManager.PlayMusic(mainMenuTheme);
    }

    public void StartGame()
    {
        GeneralManager.Instance.GoToLevelSelect();
    }

    public void Credits()
    {
        GeneralManager.Instance.ButtonPressedSFX();
        creditsMenu.SetActive(true);
    }

    public void CreditsClose()
    {
        GeneralManager.Instance.ButtonPressedSFX();
        creditsMenu.SetActive(false);
    }

    public void QuitGame()
    {
        GeneralManager.Instance.Quit();
    }

    public void OpenTutorial(bool cond)
    {
        GeneralManager.Instance.ButtonPressedSFX();
        tutorialMenu.SetActive(cond);
    }


}
