using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : PopupMenu
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

    public override void OnGoToLevelSelect()
    {
        base.OnGoToLevelSelect();
        gameObject.SetActive(false);
    }

    public override void OnRestartLevel()
    {
        base.OnRestartLevel();
        gameObject.SetActive(false);
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
