using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The static audio manager
/// </summary>
public static class AudioManager
{
    #region Fields and Properties

    static bool initialized = false;

    static GameAudioSource gameAudioSource;

    static Dictionary<AudioClipName, AudioClip> soundClips =
        new Dictionary<AudioClipName, AudioClip>();
    static Dictionary<AudioClipName, AudioClip> musicStartClips = 
        new Dictionary<AudioClipName, AudioClip>();
    static Dictionary<AudioClipName, AudioClip> musicLoopClips =
        new Dictionary<AudioClipName, AudioClip>();

    static float soundVolume = 1f;
    public static float SoundVolume { get { return soundVolume; } }
    static float musicVolume = 0.2f;
    public static float MusicVolume { get { return musicVolume; } }

    /// <summary>
    /// Gets whether or not the audio manager has been initialized
    /// </summary>
    public static bool Initialized
    {
        get { return initialized; }
    }

    #endregion

    #region Public Static Methods
    /// <summary>
    /// Initializes the audio manager
    /// </summary>
    /// <param name="soundSource">audio source</param>
    public static void Initialize(GameAudioSource source)
    {
        initialized = true;
        gameAudioSource = source;

        // Load resources, using filenames in AudioClipNames enum

        foreach (AudioClipName clip in AudioClipName.GetValues(typeof(AudioClipName)))
        {
            if (!clip.ToString().StartsWith("Music_"))
            {
                soundClips.Add(clip, Resources.Load<AudioClip>(@"Audio\Sound\" + clip.ToString()));
                //Debug.Log("Loaded: " + clip.ToString());
            }
            else
            {
                // Most music clips include a starter (intro) clip and a looped clip
                musicStartClips.Add(clip, Resources.Load<AudioClip>(@"Audio\Music\" + clip.ToString() + "_start"));
                //Debug.Log("Loaded: " + clip.ToString() + "_start");

                musicLoopClips.Add(clip, Resources.Load<AudioClip>(@"Audio\Music\" + clip.ToString() + "_loop"));
                //Debug.Log("Loaded: " + clip.ToString() + "_loop");
            }
        }

        // Load Player Volume Preferences
        if (PlayerPrefs.HasKey(PlayerPrefsKeys.SoundVolume.ToString()))
        {
            soundVolume = PlayerPrefs.GetFloat(PlayerPrefsKeys.SoundVolume.ToString());
        }
        if (PlayerPrefs.HasKey(PlayerPrefsKeys.MusicVolume.ToString()))
        {
            musicVolume = PlayerPrefs.GetFloat(PlayerPrefsKeys.MusicVolume.ToString());
        }
        gameAudioSource.SetSoundVolume(soundVolume);
        gameAudioSource.SetMusicVolume(musicVolume);
    }

    // Set Volume for Sound and Music
    public static void SetSoundVolume(float percent)
    {
        percent = percent == 0.001f ? 0f : percent;
        soundVolume = percent;
        gameAudioSource.SetSoundVolume(percent);
    }
    public static void SetMusicVolume(float percent)
    {
        percent = percent == 0.001f ? 0f : percent;
        musicVolume = percent;
        gameAudioSource.SetMusicVolume(percent);
    }

    // Special case music
    public static void Intro()
    {
        gameAudioSource.PlayMusic(soundClips[AudioClipName.Opening_Theme_1]);
    }
    public static void EndGame()
    {
        gameAudioSource.PlayMusic(soundClips[AudioClipName.Rest_In_Peace]);
    }

    // Used anytime a menu option is chosen
    public static void Chirp()
    {
        string nameString = "MenuChirp";
        //nameString = nameString.Substring(0, nameString.Length - 1);
        int chirpNum = UnityEngine.Random.Range(1, 6);
        nameString += chirpNum;
        AudioClipName name = (AudioClipName)Enum.Parse(typeof(AudioClipName), nameString);
        AudioClip clip = soundClips[name];

        gameAudioSource.PlaySound(clip);
    }

    // Special chirp for Close menu items
    public static void Close()
    {
        gameAudioSource.PlaySound(soundClips[AudioClipName.MenuChirp0]);
    }
    
    // Play sound or music
    public static void PlaySound(AudioClipName name, float volume = 1f)
    {
        gameAudioSource.PlaySound(soundClips[name], volume);
    }
    public static void PlayMusic(AudioClipName name)
    {
        gameAudioSource.PlayMusic(musicStartClips[name], musicLoopClips[name]);
    }

    #endregion

}
