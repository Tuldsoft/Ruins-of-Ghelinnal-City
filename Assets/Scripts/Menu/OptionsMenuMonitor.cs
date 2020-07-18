using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to the prefabOptionsMenu. Shows options for sound volume, music volume, 
/// and enable cheats. This info is stored in PlayerPrefs, so that it persists 
/// between play sessions
/// </summary>
public class OptionsMenuMonitor : MonoBehaviour
{
    // Toggle object for cheats
    [SerializeField]
    Toggle cheatToggle = null;

    // Label for a showing numerical value of volumes
    [SerializeField]
    Text soundVolumePercentText = null, musicVolumePercentText = null;

    // Slider for adjusting volumes
    [SerializeField]
    Slider soundVolumeSlider = null, musicVolumeSlider = null;

    // Start is called before the first frame update
    private void Start()
    {
        // PlayerPrefs are retrieved when BattleLoader initializes

        // Set cheatToggle.isOn based on BattleLoader.IsCheating
        bool cheating = BattleLoader.IsCheating;
        cheatToggle.isOn = cheating;

        // Include this third line because toggling the isOn property invokes a click event.
        BattleLoader.IsCheating = cheating;

        // PlayerPrefs for sound and music are retrieved when AudioManager initializes
        soundVolumeSlider.value = AudioManager.SoundVolume;
        musicVolumeSlider.value = AudioManager.MusicVolume * 2; // Unity plays music too loud

    }

    // Sets a new value for sound volume. Called by SoundVolumeSlider.On_Value_Changed()
    public void SetSoundVolume(float sliderValue)
    {
        // Retrieve the slider value and send it to the AudioManager
        sliderValue = sliderValue == 0.001f ? 0 : sliderValue;
        AudioManager.SetSoundVolume(sliderValue);
        int percent = Mathf.CeilToInt(sliderValue * 100);
        soundVolumePercentText.text = percent.ToString() + "%";
    }

    // Sets a new value for music volume. Called by MusicVolumeSlider.On_Value_Changed()
    public void SetMusicVolume (float sliderValue)
    {
        // Retrieve the slider value and send it to the AudioManager
        sliderValue = sliderValue == 0.001f ? 0 : sliderValue;
        AudioManager.SetMusicVolume(sliderValue / 2);
        int percent = Mathf.CeilToInt(sliderValue * 100);
        musicVolumePercentText.text = percent.ToString() + "%";
    }

    // Set current options and closes the options menu. Close button On_Click()
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

    // Called by clicking the cheat Toggle, On_Value_Changed()
    public void ToggleCheat (bool isOn)
    {
        BattleLoader.IsCheating = isOn;
    }
}
