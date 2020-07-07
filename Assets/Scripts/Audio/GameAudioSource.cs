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
        // make sure we only have one of this game object
        // in the game
        if (!AudioManager.Initialized)
        {
            GameObject child = new GameObject("SoundPlayer");
            child.transform.parent = gameObject.transform;
            soundSource = child.AddComponent<AudioSource>();

            child = new GameObject("MusicPlayer");
            child.transform.parent = gameObject.transform;
            musicSource = child.AddComponent<AudioSource>();

            child = new GameObject("MusicPlayer - Single");
            child.transform.parent = gameObject.transform;
            musicSource_Single = child.AddComponent<AudioSource>();

            musicSource.loop = true;
            musicSource.volume = 0.2f;
            musicSource_Single.loop = false;
            musicSource_Single.volume = 0.2f;

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
    /// Plays the audio clip with the given name
    /// </summary>
    /// <param name="name">name of the audio clip to play</param>
    public void PlaySound(AudioClip clip, float volume = 1.0f)
    {
        soundSource.PlayOneShot(clip, volume);
    }

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
