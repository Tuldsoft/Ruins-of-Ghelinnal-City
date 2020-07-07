using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuMonitor : MonoBehaviour
{
    [SerializeField]
    Toggle cheatToggle = null;

    [SerializeField]
    Text soundVolumePercentText = null, musicVolumePercentText = null;

    [SerializeField]
    Slider soundVolumeSlider = null, musicVolumeSlider = null;


    private void Start()
    {
        
        // PlayerPrefs are retrieved when BattleLoader initializes

        bool cheating = BattleLoader.IsCheating;
        
        cheatToggle.isOn = cheating;
        // include this third line because toggling the isOn property invokes a click event.
        BattleLoader.IsCheating = cheating;

        // PlayerPrefs for sound and music are retrieved when AudioManager initializes
        soundVolumeSlider.value = AudioManager.SoundVolume;
        musicVolumeSlider.value = AudioManager.MusicVolume * 2;

    }
    public void SetSoundVolume(float sliderValue)
    {
        sliderValue = sliderValue == 0.001f ? 0 : sliderValue;
        AudioManager.SetSoundVolume(sliderValue);
        int percent = Mathf.CeilToInt(sliderValue * 100);
        soundVolumePercentText.text = percent.ToString() + "%";
    }

    public void SetMusicVolume (float sliderValue)
    {
        sliderValue = sliderValue == 0.001f ? 0 : sliderValue;
        AudioManager.SetMusicVolume(sliderValue / 2);
        int percent = Mathf.CeilToInt(sliderValue * 100);
        musicVolumePercentText.text = percent.ToString() + "%";
    }

    public void Click_Close()
    {
        AudioManager.Close();
        int isCheating = cheatToggle.isOn ? 0 : -1;
        PlayerPrefs.SetInt(PlayerPrefsKeys.IsCheating.ToString(), isCheating);
        PlayerPrefs.SetFloat(PlayerPrefsKeys.SoundVolume.ToString(), AudioManager.SoundVolume);
        PlayerPrefs.SetFloat(PlayerPrefsKeys.MusicVolume.ToString(), AudioManager.MusicVolume);

        PlayerPrefs.Save();
        
        Destroy(gameObject);
    }

    public void ToggleCheat (bool isOn)
    {
        BattleLoader.IsCheating = isOn;
    }
}
