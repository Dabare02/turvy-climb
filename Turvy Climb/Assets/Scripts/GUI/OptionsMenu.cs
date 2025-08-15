using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Slider masterVolume;
    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider sfxVolume;

    private void Start()
    {
        masterVolume.value = GeneralManager.Instance.audioManager.GetMasterVolume();
        musicVolume.value = GeneralManager.Instance.audioManager.GetMusicVolume();
        sfxVolume.value = GeneralManager.Instance.audioManager.GetSFXVolume();
    }

    public void OnResume()
    {
        GeneralManager.Instance.pause = false;
    }

    public void OnGoToLevelSelect()
    {
        GeneralManager.Instance.pause = false;

        LevelManager lvlManager = FindObjectOfType<LevelManager>();
        if (lvlManager != null) lvlManager.GoBackToLevelSelect();
        else GeneralManager.Instance.GoToLevelSelect();
        
    }

    public void OnRestartLevel()
    {
        GeneralManager.Instance.pause = false;
        
        LevelManager lvlManager = FindObjectOfType<LevelManager>();
        if (lvlManager != null)
        {
            lvlManager.RestartLevel();
            Debug.LogError("LevelManager not found.");
        } else GeneralManager.Instance.RestartLevel();
    }

    public void OnMasterVolumeChanged()
    {
        GeneralManager.Instance.audioManager.SetMasterVolume(masterVolume.value);
    }

    public void OnMusicVolumeChanged()
    {
        GeneralManager.Instance.audioManager.SetMusicVolume(musicVolume.value);
    }
    
    public void OnSFXVolumeChanged()
    {
        GeneralManager.Instance.audioManager.SetSFXVolume(sfxVolume.value);
    }
}
