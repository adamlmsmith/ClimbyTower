using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public AudioMixer m_AudioMixer;
    public AudioSource efxSource;
    public AudioSource musicSource;
    public static SoundManager instance = null;

    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;

    public AudioMixerGroup SoundEffectsMixerGroup;
    public AudioMixerGroup GameSoundEffectsMixerGroup;

    public float MusicVolume
    {
        get
        {
            float musicVolume;
            m_AudioMixer.GetFloat("MusicVolume", out musicVolume);
            return musicVolume;
        }

        set
        {
            m_AudioMixer.SetFloat ("MusicVolume", value);
            PlayerPrefs.SetFloat ("MusicVolume", value);
        }
    }

    public float EffectsVolume
    {
        get
        {
            float effectsVolume;
            m_AudioMixer.GetFloat("EffectsVolume", out effectsVolume);
            return effectsVolume;
        }
        
        set
        {
            m_AudioMixer.SetFloat ("EffectsVolume", value);
            PlayerPrefs.SetFloat ("EffectsVolume", value);
        }
    }

    public float GameEffectsVolume
    {
        get
        {
            float gameEffectsVolume;
            m_AudioMixer.GetFloat("GameSFXVolume", out gameEffectsVolume);
            return gameEffectsVolume;
        }
        
        set
        {
            m_AudioMixer.SetFloat ("GameSFXVolume", value);
        }
    }

	void Awake () 
    {
	    if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy (gameObject);

        DontDestroyOnLoad (gameObject);
	}

    void Start()
    {
        m_AudioMixer.SetFloat ("MusicVolume", PlayerPrefs.GetFloat ("MusicVolume"));
        m_AudioMixer.SetFloat ("EffectsVolume", PlayerPrefs.GetFloat ("EffectsVolume"));
    }
	
    public void PlaySingle(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play ();
    }

    public void RandomizeSfx(params AudioClip [] clips)
    {
        int randomIndex = Random.Range (0, clips.Length);
        float randomPitch = Random.Range (lowPitchRange, highPitchRange);

        efxSource.pitch = randomPitch;
        efxSource.clip = clips [randomIndex];
        efxSource.Play ();
    }
}
