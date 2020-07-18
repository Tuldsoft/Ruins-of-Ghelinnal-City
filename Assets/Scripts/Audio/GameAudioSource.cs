using UnityEngine;

/// <summary>
/// An audio source for the entire game
/// </summary>
public class GameAudioSource : MonoBehaviour
{
    AudioSource soundSource;
    AudioSource musicSource;
    AudioSource musicSource_Single;
    
    void Awake()
	{
        // make sure we only have one of this game object in the game
        if (!AudioManager.Initialized)
        {
            
            // Create audiosource for Sound
            GameObject child = new GameObject("SoundPlayer");
            child.transform.parent = gameObject.transform;
            soundSource = child.AddComponent<AudioSource>();

            // Create audiosource for Music
            child = new GameObject("MusicPlayer");
            child.transform.parent = gameObject.transform;
            musicSource = child.AddComponent<AudioSource>();

            // Create audiosource for Music - Single
            // Used when a music file is played only once
            child = new GameObject("MusicPlayer - Single");
            child.transform.parent = gameObject.transform;
            musicSource_Single = child.AddComponent<AudioSource>();

            // Set music looping
            musicSource.loop = true;
            musicSource.volume = 0.2f;
            musicSource_Single.loop = false;
            musicSource_Single.volume = 0.2f;

            // Send this class to the AudioManager
            AudioManager.Initialize(this);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // duplicate game object, so destroy
            Destroy(gameObject);
        }
    }


    /// <summary>
    /// Plays the specified sound clip
    /// </summary>
    /// <param name="clip">Unity clip</param>
    /// <param name="volume">Volume level 0.0 to 1.0</param>
    public void PlaySound(AudioClip clip, float volume = 1.0f)
    {
        soundSource.PlayOneShot(clip, volume);
    }

    /// <summary>
    /// Play music, either looping or singlely.
    /// Looped music starts with a intro clip, then a looped clip
    /// </summary>
    /// <param name="startClip"></param>
    /// <param name="loopClip"></param>
    public void PlayMusic(AudioClip startClip, AudioClip loopClip = null)
    {
        musicSource_Single.Stop();
        musicSource.Stop();

        musicSource_Single.clip = startClip;
        musicSource_Single.Play();
        
        if (loopClip != null)
        {
            musicSource.clip = loopClip;
            musicSource.PlayScheduled(AudioSettings.dspTime + startClip.length);
        }
        
    }

    // Set volume for sound or music
    public void SetSoundVolume(float volume)
    {
        soundSource.volume = volume;
    }
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        musicSource_Single.volume = volume;
    }
}
