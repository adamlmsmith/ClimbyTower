/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager
{
    [System.Flags]
    public enum AudioFlags
    {
        NONE					 = 0,
        UNSTACKABLE				 = 1 << 0,
        LOOPING					 = 1 << 1,
        UNINTERRUPTABLE			 = 1 << 2,
        MUSIC					 = 1 << 3,
        IGNORE_LISTENER_PAUSE	 = 1 << 4
    }

    class FadingSound
    {
        public string ClipToFade;
        public float ElapsedTime;
        public float FadeTime;
        public float StartingVolume;
        public float FinalVolume;
    }

    class SoundData
    {
        public GameObject SoundObject = null;
        public float ScaledVolume = 1.0f;
        public float Volume = 1.0f;
        public bool IsMusic = false;
        public bool IsPaused = false;

        public bool Is(string name)
        {
            if (SoundObject == null) return false;
            return SoundObject.name == name;
        }
        public bool Is(int id)
        {
            if (SoundObject == null) return false;
            return SoundObject.GetInstanceID() == id;
        }
        public bool IsPlaying
        {
            get
            {
                if (SoundObject == null) return false;
                if (SoundObject.GetComponent<AudioSource>() == null) return false;
                return SoundObject.GetComponent<AudioSource>().isPlaying;
            }
        }            
    }

    //--------------------------------------------------------------------------------
    List<AudioClip> Clips = new List<AudioClip>();

    // Hold the game objects that represent active and paused sounds
    List<SoundData> m_Sounds = new List<SoundData>();

    List<FadingSound> FadingSounds = new List<FadingSound>();

    static AudioManager m_Instance = null;

    // This GameObject is used as a parent for all created sound objects and is useful
    // because at any given point you can see what sounds are playing
    GameObject m_SoundContainer = null;

    /// <summary>
    /// Enables a verbose mode with debug printouts
    /// </summary>
    static bool m_VerboseMode = false;

    bool m_MusicEnabled = true;
    bool m_SFXEnabled = true;

    // properties
    public bool VerboseMode { get { return m_VerboseMode; } set { m_VerboseMode = value; } }
    public bool MusicEnabled { get { return m_MusicEnabled; } set { EnableMusic(value); } }
    public bool SFXEnabled { get { return m_SFXEnabled; } set { EnableSFX(value); } }

    public float GlobalMusicScalar = 1.0f;
    public float GlobalSoundFXScalar = 1.0f;

    //--------------------------------------------------------------------------------
    // Singleton Access
    //--------------------------------------------------------------------------------
    public static AudioManager GetInstance()
    {
        if (null == m_Instance)
        {
            m_Instance = new AudioManager();
            m_Instance.Init();
        }

        return m_Instance;
    }

    public static void DestroyInstance()
    {
        m_Instance = null;
    }

    public void Update()
    {
        if (FadingSounds.Count > 0)
        {
            Fade();
        }
    }
    /// <summary>
    /// called every frame to handle fading of sounds
    /// </summary>
    void Fade()
    {
        for (int fadedIndex = 0; fadedIndex < FadingSounds.Count; fadedIndex++)
        {
            FadingSounds[fadedIndex].ElapsedTime += Time.deltaTime;
            if (FadingSounds[fadedIndex].ElapsedTime / FadingSounds[fadedIndex].FadeTime <= 1.0f)
                SetVolume(FadingSounds[fadedIndex].ClipToFade, Lerp(FadingSounds[fadedIndex].StartingVolume, FadingSounds[fadedIndex].FinalVolume, FadingSounds[fadedIndex].ElapsedTime / FadingSounds[fadedIndex].FadeTime));
            else
            {
                FadingSounds.RemoveAt(fadedIndex);
                fadedIndex--;
            }
        }
    }

    /// <summary>
    /// helper function for lerping
    /// </summary>
    float Lerp(float startValue, float finalValue, float weight)
    {
        return (startValue + ((finalValue - startValue) * weight));
    }
    //--------------------------------------------------------------------------------
    // Exposed Methods
    //--------------------------------------------------------------------------------
    // Plays the clip (also adds it if it hasn't been played yet)
    // \return  Unique ID of that GameObject just in case you want to store it and later
    //          manipulate this individual instance of a clip (useful if also stackable)
    // bInterruptable works in collaboration with bStackable. If bStackable is false and bInterruptable is false, calling play on the sound while it is already playing
    // will be ignored
    public int PlayClip(string name, float volume = 1.0f, AudioFlags audioFlags = AudioFlags.NONE)
    {
        bool stackable = ((audioFlags & AudioFlags.UNSTACKABLE) != AudioFlags.UNSTACKABLE);
        bool loop = ((audioFlags & AudioFlags.LOOPING) == AudioFlags.LOOPING);
        bool interruptable = ((audioFlags & AudioFlags.UNINTERRUPTABLE) != AudioFlags.UNINTERRUPTABLE);
        bool musicTrack = ((audioFlags & AudioFlags.MUSIC) == AudioFlags.MUSIC);
        bool ignoreListenerPause = ((audioFlags & AudioFlags.IGNORE_LISTENER_PAUSE) == AudioFlags.IGNORE_LISTENER_PAUSE);

        // We have to use this function because of passing delays all the way down to the observer
        return InternalPlayClip(name, volume, stackable, loop, interruptable, musicTrack, ignoreListenerPause);
    }

    //--------------------------------------------------------------------------------
    // Access to clips by NAME
    //--------------------------------------------------------------------------------
    // Stop a sound/stackable sounds in the list of game objects that represent clips
    // Also remove them from the list of game objects and destroy the game objects
    public void StopClip(string name)
    {
        List<SoundData> StopThese = m_Sounds.FindAll(c => c.Is(name));

        // Safety check
        if (StopThese.Count <= 0)
        {
            //Debug.LogWarning("AudioManager::StopClip() - No clips to stop with the name: " + sName);
            return;
        }

        // Stop all sounds and make sure they are removed from the AudioClips list and
        // the corresponding game objects are destroyed
        foreach (SoundData obj in StopThese)
        {
            obj.SoundObject.GetComponent<AudioSource>().Stop();
            m_Sounds.Remove(obj);
            GameObject.Destroy(obj.SoundObject);
        }
    }
    //--------------------------------------------------------------------------------
    // Pause a sound/stackable sounds in the list of game objects that represent clips
    public bool PauseClip(string name)
    {
        // Pause a sound/stackable sounds in the list of game objects that represent clips
        List<SoundData> pauseThese = m_Sounds.FindAll(c => c.Is(name) && c.IsPlaying);

        // Safety check
        if (pauseThese.Count <= 0)
        {
            Debug.LogWarning("AudioManager::PauseClip() - No clips to pause with the name: " + name);
            return false;
        }

        // Pause all sounds of this name
        foreach (SoundData obj in pauseThese)
        {
            obj.SoundObject.GetComponent<AudioSource>().Pause();
            obj.IsPaused = true;
        }
        return true;
    }
    //--------------------------------------------------------------------------------
    // Unpause a sound/stackable sounds in the list of game objects that represent clips
    public bool UnpauseClip(string name)
    {
        // Pause a sound/stackable sounds in the list of game objects that represent clips
        List<SoundData> unpauseThese = m_Sounds.FindAll(c => c.Is(name) && c.IsPaused);

        // Safety check
        if (unpauseThese.Count <= 0)
        {
            Debug.LogWarning("AudioManager::UnpauseClip() - No clips to unpause with the name: " + name);
            return false;
        }

        // Pause all sounds of this name
        foreach (SoundData obj in unpauseThese)
        {
            obj.SoundObject.GetComponent<AudioSource>().Play();
            obj.IsPaused = false;
        }
        return true;
    }
    //--------------------------------------------------------------------------------
    // Adjust the volume on a sound/stackable sounds in the list of game objects that represent clips
    public void SetVolume(string name, float volume)
    {
        // Safety checks
        if (volume < 0.0f)
        {
            volume = 0.0f;
            Debug.LogWarning("AudioManager::SetVolume() - Volume param was negative, making it 0.0f");
        }
        else if (volume > 1.0f)
        {
            volume = 1.0f;
            Debug.LogWarning("AudioManager::SetVolume() - Volume param was larger than 1.0f, making it 1.0f");
        }

        // Adjust the volume on a sound/stackable sounds in the list of game objects that represent clips
        List<SoundData> volumizeThese = m_Sounds.FindAll(c => c.Is(name));

        // Safety check
        if (volumizeThese.Count <= 0)
        {
            Debug.LogWarning("AudioManager::SetVolume() - No clips to adjust volume on with the name: " + name);
            return;
        }

        // Adjust volume in all sounds
        foreach (SoundData obj in volumizeThese)
        {
            // If the sound is active and enabled, set its volume
            if ((obj.IsMusic && MusicEnabled) ||
                (!obj.IsMusic && SFXEnabled))
            {
                //GAME_TODO how do we want this to work
                if (obj.IsMusic)
                    volume *= GlobalMusicScalar;
                else
                    volume *= GlobalSoundFXScalar;

                obj.SoundObject.GetComponent<AudioSource>().volume = volume;
            }

            // In addition to that, save that new volume in case that type of audio is muted
            // So if it is unmuted, it will have the correct volume
            obj.ScaledVolume = volume;
        }
    }
    //--------------------------------------------------------------------------------
    // Adjust the pitch of a sound/stackable sounds in the list of game objects that represent clips
    public void SetPitch(string name, float pitch)
    {
        // Safety checks
        if (pitch < 0.0f)
        {
            pitch = 0.0f;
            Debug.LogWarning("AudioManager::SetPitch() - Pitch param was negative, making it 0.0f");
        }

        // Adjust the pitch of a sound/stackable sounds in the list of game objects that represent clips
        List<SoundData> pitchitizeThese = m_Sounds.FindAll(c => c.Is(name));

        // Safety check
        if (pitchitizeThese.Count <= 0)
        {
            Debug.LogWarning("AudioManager::SetPitch() - No clips to set pitch on with the name: " + name);
            return;
        }

        // Adjust pitch of all sounds
        foreach (SoundData obj in pitchitizeThese)
            obj.SoundObject.GetComponent<AudioSource>().pitch = pitch;
    }

    //--------------------------------------------------------------------------------
    // Access to clips by UNIQUE ID
    //--------------------------------------------------------------------------------
    public void StopClip(int id)
    {
        SoundData obj = m_Sounds.Find(c => c.Is(id));

        // Safety check
        if (obj == null)
        {
            Debug.LogWarning("AudioManager::StopClip() - No clip to stop with the id: " + id);
            return;
        }

        // Stop this unique clip
        obj.SoundObject.GetComponent<AudioSource>().Stop();
        m_Sounds.Remove(obj);
        GameObject.Destroy(obj.SoundObject);
    }
    //--------------------------------------------------------------------------------
    public void PauseClip(int id)
    {
        SoundData obj = m_Sounds.Find(c => c.Is(id) && c.IsPlaying);

        // Safety check
        if (obj == null)
        {
            Debug.LogWarning("AudioManager::PauseClip() - No clip to pause with the id: " + id);
            return;
        }

        obj.SoundObject.GetComponent<AudioSource>().Pause();
        obj.IsPaused = true;
    }
    //--------------------------------------------------------------------------------
    public bool UnpauseClip(int id)
    {
        SoundData obj = m_Sounds.Find(c => c.Is(id) && c.IsPaused);

        // Safety check
        if (obj == null)
        {
            Debug.LogWarning("AudioManager::UnpauseClip() - No clip to unpause with the id: " + id);
            return false;
        }

        obj.SoundObject.GetComponent<AudioSource>().Play();
        obj.IsPaused = false;
        return true;
    }
    //--------------------------------------------------------------------------------
    public void SetGlobalVolumeScale(float volumeScale, bool isMusic)
    {
        if (isMusic)
            GlobalMusicScalar = volumeScale;
        else
            GlobalSoundFXScalar = volumeScale;

        CleanSounds();

        foreach (SoundData obj in m_Sounds)
        {
            if (obj.IsMusic && isMusic)
            {
                obj.ScaledVolume = obj.Volume * volumeScale;
                obj.SoundObject.GetComponent<AudioSource>().volume = obj.ScaledVolume;
            }
            else if (!obj.IsMusic && !isMusic)
            {
                obj.ScaledVolume = obj.Volume * volumeScale;
                obj.SoundObject.GetComponent<AudioSource>().volume = obj.ScaledVolume;
            }
        }
    }
    //--------------------------------------------------------------------------------
    public void SetVolume(int id, float volume)
    {
        // Safety checks
        if (volume < 0.0f)
        {
            volume = 0.0f;
            Debug.LogWarning("AudioManager::SetVolume() - Volume param was negative, making it 0.0f");
        }
        else if (volume > 1.0f)
        {
            volume = 1.0f;
            Debug.LogWarning("AudioManager::SetVolume() - Volume param was larger than 1.0f, making it 1.0f");
        }

        SoundData obj = m_Sounds.Find(c => c.Is(id));

        // Safety check
        if (obj == null)
        {
            Debug.LogWarning("AudioManager::SetVolume() - No clip to adjust volume on with the in: " + id);
            return;
        }

        //GAME_TODO how do we want this to work?
        if (obj.IsMusic)
            volume *= GlobalMusicScalar;
        else
            volume *= GlobalSoundFXScalar;

        obj.SoundObject.GetComponent<AudioSource>().volume = volume;
    }
    //--------------------------------------------------------------------------------
    public void SetPitch(int id, float pitch)
    {
        // Safety checks
        if (pitch < 0.0f)
        {
            pitch = 0.0f;
            Debug.LogWarning("AudioManager::SetPitch() - Pitch param was negative, making it 0.0f");
        }

        SoundData obj = m_Sounds.Find(c => c.Is(id));

        // Safety check
        if (obj == null)
        {
            Debug.LogWarning("AudioManager::SetPitch() - No clip to set pitch on with the id: " + id);
            return;
        }

        obj.SoundObject.GetComponent<AudioSource>().pitch = pitch;
    }
    //-----------------------------------------------------------------------------------
    public void EnableSFX(bool enabled)
    {
        CleanSounds();

        foreach (SoundData obj in m_Sounds)
        {
            if (!obj.IsMusic)
            {
                if (enabled)
                    obj.SoundObject.GetComponent<AudioSource>().volume = obj.ScaledVolume;
                else
                    obj.SoundObject.GetComponent<AudioSource>().volume = 0.0f;
            }
        }

        m_SFXEnabled = enabled;
    }

    public void EnableMusic(bool enabled)
    {
        CleanSounds();

        foreach (SoundData obj in m_Sounds)
        {
            if (obj.IsMusic)
            {
                if (enabled)
                    obj.SoundObject.GetComponent<AudioSource>().volume = obj.ScaledVolume;
                else
                    obj.SoundObject.GetComponent<AudioSource>().volume = 0.0f;
            }
        }

        m_MusicEnabled = enabled;
    }

    /// <summary>
    /// Used to set the data for the sound to fade in or out.
    /// </summary>
    /// <param name="name">Name of the sound</param>
    /// <param name="fadeTime">The time you wish to fade by, volume will be at final volume after this many seconds</param>
    /// <param name="startingVolume">Volume the sound begins at</param>
    /// <param name="finalVolume">Final volume the sound ends at</param>
    public void SetFade(string name, float fadeTime, float startingVolume = 1.0f, float finalVolume = 0.0f)
    {
        if (!IsClipPlaying(name))
            return;

        FadingSound newSoundToFade = new FadingSound();

        //GAME_TODO might need to add scalar here too
        newSoundToFade.ClipToFade = name;
        newSoundToFade.FadeTime = fadeTime;
        newSoundToFade.StartingVolume = startingVolume;
        newSoundToFade.FinalVolume = finalVolume;
        newSoundToFade.ElapsedTime = 0.0f;
        FadingSounds.Add(newSoundToFade);
    }

    public void FadeOutAllSounds(float fadeTime)
    {
        foreach (SoundData obj in m_Sounds)
        {
            if (obj.IsPlaying)
            {
                SetFade(obj.SoundObject.name, fadeTime, obj.SoundObject.GetComponent<AudioSource>().volume, 0.0f);
            }
        }
    }

    public void FadeInAllSounds(float fadeTime)
    {
        foreach (SoundData obj in m_Sounds)
        {
            if (obj.IsPlaying)
            {
                SetFade(obj.SoundObject.name, fadeTime, 0.0f, 1.0f);
            }
        }
    }

    //--------------------------------------------------------------------------------
    // Access to all clips
    //--------------------------------------------------------------------------------
    // Pause all stackable/non-stackable sounds
    public void PauseAllSounds()
    {
        foreach (SoundData obj in m_Sounds)
        {
            if (obj.IsPlaying)
            {
                if (obj.IsPaused == false)
                {
                    obj.SoundObject.GetComponent<AudioSource>().Pause();
                    obj.IsPaused = true;
                }
            }
        }
    }
    //--------------------------------------------------------------------------------
    // Unpause all stackable/non-stackable sounds
    public void UnpauseAllSounds()
    {
        foreach (SoundData obj in m_Sounds)
        {
            if (obj.IsPaused)
            {
                obj.SoundObject.GetComponent<AudioSource>().Play();
                obj.IsPaused = false;
            }
        }
    }
    //--------------------------------------------------------------------------------
    // Stop all stackable/non-stackable sounds
    public void StopAllSounds()
    {
        // Stop everything
        foreach (SoundData obj in m_Sounds)
        {
            if (obj.SoundObject != null)
            {
                obj.SoundObject.GetComponent<AudioSource>().Stop();
                GameObject.Destroy(obj.SoundObject);
            }
        }
        m_Sounds.Clear();
    }
    //--------------------------------------------------------------------------------
    // Used by the class below to remove a sound
    internal void RemoveSound(int id)
    {
        for (int i = 0; i < m_Sounds.Count; i++)
        {
            if (m_Sounds[i].Is(id))
            {
                m_Sounds.RemoveAt(i);
                break;
            }
        }
    }
    void CleanSounds()
    {
        var remove = m_Sounds.FindAll(c => c.SoundObject == null);
        foreach (var s in remove)
            m_Sounds.Remove(s);
    }

    //--------------------------------------------------------------------------------
    public bool IsClipPlaying(string name)
    {
        return m_Sounds.Find(c => c.Is(name)) != null;
    }
    //--------------------------------------------------------------------------------
    public bool IsClipPaused(string name)
    {
        return m_Sounds.Find(c => c.Is(name) && c.IsPaused) != null;
    }

    public bool IsClipPaused(int id)
    {
        return m_Sounds.Find(c => c.Is(id) && c.IsPaused) != null;
    }

    //--------------------------------------------------------------------------------
    // Internal Methods
    //--------------------------------------------------------------------------------
    // Loads clips dynamically from Resources/Sounds/
    internal void Init()
    {
        if (m_SoundContainer == null)
            m_SoundContainer = GameObject.Find("SoundContainer");

        if (m_SoundContainer == null)
        {
            m_SoundContainer = new GameObject("SoundContainer");
            Object.DontDestroyOnLoad(m_SoundContainer);
        }

        // Dynamically load all clips from the Resources/Sounds/ directory
        Object[] rawClips = Resources.LoadAll("Sounds", typeof(AudioClip));

        // Safety check
        if (0 == rawClips.GetLength(0))
        {
            Debug.LogWarning("AudioManager::Init() - No raw clip assets found. Make sure they are in Resources/Common/Sounds/");
            return;
        }

        // Store references to each of these raw clips
        foreach (AudioClip clip in rawClips)
            Clips.Add(clip);

		// TODO read/write this to local data
//            var game_id = Common.SettingsManager.Get<System.String>("game_id");
//            float soundScale = Common.SettingsManager.Get<System.Single>(game_id + ".music_volume");
//            SetGlobalVolumeScale(soundScale, true);
//            
//            soundScale = Common.SettingsManager.Get<System.Single>(game_id + ".sound_volume");
//            SetGlobalVolumeScale(soundScale, false);
        
        // This used to do a check in CrossPlatformData for a flag
        EnableMusic(true);
    }

    void ClearSounds()
    {
        m_Sounds.Clear();
        FadingSounds.Clear();
    }

	public bool AudioClipExists(string audioClipName)
	{
		return (null != Clips.Find(c => c.name == audioClipName));
	}
    //--------------------------------------------------------------------------------
    // Creates and tracks a new GameObject that represents the clip to be played
    // NOTE: GameObjects are created for two reasons: so it's easy to see when pausing
    //       the game, etc. what sounds are playing/have been played, and also to handle 
    //       properties that aren't part of a clip like stackability, etc. in one object
    // \return The unique ID of the sound to play
    int AddClip(string sName, AudioClip clip, bool bLoop, float fVolume, bool bMusicTrack, bool bIgnoreListenerPause)
    {
        // Safety check params
        // NOTE: Making sure we're not adding the same clip twice happens above this in
        // PlaySound() where appropriate since only non-stackable sounds are limit by that
        if (null == clip || "" == sName || fVolume < 0.0f)
        {
            Debug.LogError("AudioManager::AddClip() - Problem adding clip " + sName +
                ". Check name, make sure raw clip is in Resources/Sounds/, and volume level is positive.");
            return -1;
        }

        // Create the new game object representing this clip and setup properties
        GameObject obj = new GameObject(sName);
        obj.AddComponent<AudioSource>();
        obj.GetComponent<AudioSource>().clip = clip;
        obj.GetComponent<AudioSource>().playOnAwake = false;
        obj.GetComponent<AudioSource>().loop = bLoop;
        obj.GetComponent<AudioSource>().ignoreListenerPause = bIgnoreListenerPause;

        if ((bMusicTrack && m_MusicEnabled) || (!bMusicTrack && m_SFXEnabled))
        {
            if (bMusicTrack)
                obj.GetComponent<AudioSource>().volume = fVolume * GlobalMusicScalar;
            else
                obj.GetComponent<AudioSource>().volume = fVolume * GlobalSoundFXScalar;
        }
        else
            obj.GetComponent<AudioSource>().volume = 0.0f;

        // Attach the script that will clean up this object itself when it's complete
        SoundObserver observer = obj.AddComponent<SoundObserver>();
        observer.ObserveSound(obj, obj.GetComponent<AudioSource>().clip.length);

        // Attach these objects to this class as a container
        obj.transform.parent = m_SoundContainer.transform;

        // Make sure we hold on to the game object that represents this sound
        SoundData soundData = new SoundData();
        soundData.SoundObject = obj;
        soundData.IsMusic = bMusicTrack;

        if (bMusicTrack)
            soundData.ScaledVolume = fVolume * GlobalMusicScalar;
        else
            soundData.ScaledVolume = fVolume * GlobalSoundFXScalar;

        soundData.Volume = fVolume;
        m_Sounds.Add(soundData);

        return obj.GetInstanceID();
    }
    //--------------------------------------------------------------------------------
    // This is used internally only, and it's the same as the exposed Play() except it passes
    // along the delay so it can be applied when tracking stackable sounds to destroy
    int InternalPlayClip(string sName, float fVolume, bool bStackable, bool bLoop, bool bInterruptable, bool bMusicTrack, bool bIgnoreListenerPause)
    {
        if (m_VerboseMode && Debug.isDebugBuild)
            Debug.Log("PlayClip(" + sName + ")");

        // Safety checks name so default doesn't go into predicate below
        if ("" == sName)
        {
            Debug.LogError("AudioManager::PlayClip() - Clip name param was empty");
            return -1;
        }
        AudioClip clipToPlay = Clips.Find(c => c.name == sName);
        // Safety check
        if (!clipToPlay)
        {
            if (Debug.isDebugBuild)
                Debug.LogError("AudioManager::PlayClip() - " + sName + " was not found. Make sure clips are in Resources/Common/Sounds/");
            return -1;
        }

        int nReturnID = -1;

        // Add this clip if it hasn't already been played and if it isn't stackable
        // NOTE: This check prevents duplicates from being added when the sound
        // is NOT meant to be stackable and a single instance is being controlled

        if (!m_Sounds.Exists(o => o.Is(clipToPlay.name) && !bStackable))
        {
            nReturnID = AddClip(sName, clipToPlay, bLoop, fVolume, bMusicTrack, bIgnoreListenerPause);
        }

        if (IsClipPaused(sName) == true)
        {
            UnpauseClip(sName);
            return nReturnID;
        }

        // Now that it's added (or already was) play the clip
        // NOTE: If the sound is NOT stackable, it can just be played...
        if (!bStackable)
        {
            // Grab this here after we are sure its properties have been setup correctly
            SoundData obj = m_Sounds.Find(o => o.Is(clipToPlay.name));

            // Since there is only one clip with this name if NOT stackable, stop it first
            if (obj.IsPlaying && !bInterruptable)
            {
                return nReturnID;
            }
            else if (obj.IsPlaying)
            {
                obj.SoundObject.GetComponent<AudioSource>().Stop();
            }

            obj.IsMusic = bMusicTrack;

            // Make the actual call to play the sound
            obj.SoundObject.GetComponent<AudioSource>().Play();
            obj.IsPaused = false;
            obj.Volume = fVolume;

            // Return the unique id to this sound GameObject
            return nReturnID;
        }
        // ...otherwise (it IS stackable) we have to fire off a one shot sound
        // for it without stopping the other instances named the same thing
        else
        {
            nReturnID = AddClip(sName, clipToPlay, bLoop, fVolume, bMusicTrack, bIgnoreListenerPause);

            // Grab this here after we are sure its properties have been setup correctly
            SoundData obj = m_Sounds.Find(o => o.Is(nReturnID));

            // Make the actual call to play the sound
            obj.SoundObject.GetComponent<AudioSource>().Play();
            obj.IsPaused = false;
            obj.Volume = fVolume;

            // Return the unique id to this sound GameObject
            return nReturnID;
        }
    }
}


// This class is meant to be attached to each sound object. It's sole purpose
// is to be able to call coroutines to cleanup stackable sounds as they're played.
class SoundObserver : MonoBehaviour
{

    float m_Duration = 0.0f;
    AudioSource m_AudioSource;

    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// This is called by the AudioManager to pass data for duration of the clip in
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="fDuration"></param>
    public void ObserveSound(GameObject obj, float duration)
    {
        // Safety check
        if (!obj.GetComponent<AudioSource>())
        {
            Debug.LogWarning("StackableSoundObserver::ObserveSound() - GameObject passed in doesn't have an AudioSource component!");
            return;
        }

        m_Duration = duration;
    }

    /// <summary>
    /// If the sound is finished playing, destroy the game object that's holding it!
    /// </summary>
    public void Update()
    {
        if (((m_AudioSource.GetComponent<AudioSource>().loop == false) && (m_AudioSource.GetComponent<AudioSource>().time >= m_Duration))
            || (m_AudioSource.isPlaying == false
                && AudioManager.GetInstance().IsClipPaused(m_AudioSource.gameObject.GetInstanceID()) == false
                && (m_AudioSource.ignoreListenerPause == AudioListener.pause
                    || AudioListener.pause == false)))
        {
            AudioManager.GetInstance().RemoveSound(m_AudioSource.gameObject.GetInstanceID());
            GameObject.Destroy(this.gameObject);
        }
    }
}*/