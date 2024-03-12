using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

public class Climber : MonoBehaviour
{
    public enum MoveDirections { UP, DOWN, LEFT, RIGHT };
    public enum Rarity { NEVER, COMMON, RARE, EPIC, LEGENDARY };
    public string ClimberName;
    public string ClimberDisplayName;

    BuildingManager buildingManager;

    Vector2 touchStartPosition;

    public bool Unlocked;
    public Rarity ClimberRarity;

    public UnityEvent ChangedPosition;

    [Header("Audio")]
    public AudioClip m_BoneCrushClip;
    public AudioClip m_FallClip;
    public AudioClip m_DeflectGrenadeClip;
    public AudioClip m_ElectrocuteClip;
    public List<AudioClip> m_FootstepClips = new List<AudioClip>();
    private AudioSource m_ClimberAudio;
    private AudioSource m_FallAudio;
    private AudioSource m_DeflectGrenadeAudio;
    private AudioSource m_ElectrocuteAudio;
    private AudioSource m_FootStepAudio;

    bool IsAlive = true;

    List<MoveDirections> m_MoveQueue = new List<MoveDirections>();
    const int m_MaxSizeOfMoveQueue = 2;

    void Awake()
    {
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "PlayerInputSwipeLeft");
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "PlayerInputSwipeRight");
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "PlayerInputSwipeUp");
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "PlayerInputSwipeDown");
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "PlayerInputTap");

        m_ClimberAudio = gameObject.AddComponent<AudioSource>();
        m_ClimberAudio.clip = m_BoneCrushClip;
        m_ClimberAudio.outputAudioMixerGroup = SoundManager.instance.GameSoundEffectsMixerGroup;
        m_ClimberAudio.playOnAwake = false;

        m_FallAudio = gameObject.AddComponent<AudioSource>();
        m_FallAudio.clip = m_FallClip;
        m_FallAudio.volume = 0.3f;
        m_FallAudio.outputAudioMixerGroup = SoundManager.instance.SoundEffectsMixerGroup;
        m_FallAudio.playOnAwake = false;

        m_DeflectGrenadeAudio = gameObject.AddComponent<AudioSource>();
        m_DeflectGrenadeAudio.clip = m_DeflectGrenadeClip;
        m_DeflectGrenadeAudio.outputAudioMixerGroup = SoundManager.instance.GameSoundEffectsMixerGroup;
        m_DeflectGrenadeAudio.playOnAwake = false;

        m_ElectrocuteAudio = gameObject.AddComponent<AudioSource>();
        m_ElectrocuteAudio.clip = m_ElectrocuteClip;
        m_ElectrocuteAudio.outputAudioMixerGroup = SoundManager.instance.GameSoundEffectsMixerGroup;
        m_ElectrocuteAudio.playOnAwake = false;

        m_FootStepAudio = gameObject.AddComponent<AudioSource>();
        m_FootStepAudio.volume = 0.5f;
        m_FootStepAudio.outputAudioMixerGroup = SoundManager.instance.GameSoundEffectsMixerGroup;
        m_FootStepAudio.playOnAwake = false;
    }

    void Start()
    {
        IsAlive = true;
        buildingManager = GameObject.Find("Board").GetComponent<BuildingManager>(); // TODO replace this!

        m_MoveQueue.Clear();
    }

    void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKeyDown (KeyCode.D))
            TriggerDeath();

        if(Input.GetKeyDown(KeyCode.C))
            GameVariables.instance.PlayerManager.CurrentPlayer.Coins += 100;

        if(Input.GetKeyDown(KeyCode.P))
        {
           PlayerPrefs.DeleteAll();
            UnityEditor.EditorApplication.isPlaying = false;
        }
#endif

        // TODO this is really sloppy
        if(buildingManager.IsWindowClosed(GameVariables.instance.PlayerManager.CurrentPlayer.BoardPosition))
        {
            if(IsStationary())
            {
                m_ClimberAudio.clip = m_BoneCrushClip;
                m_ClimberAudio.Play();
                TriggerDeath();
            }
        }

        if (IsAlive && IsStationary())
        {
            if(m_MoveQueue.Count > 0)
            {
                switch(m_MoveQueue[0])
                {
                    case MoveDirections.UP:
                        PlayerMoveUp();
                        break;
                    case MoveDirections.DOWN:
                        PlayerMoveDown();
                        break;
                    case MoveDirections.LEFT:
                        PlayerMoveLeft();
                        break;
                    case MoveDirections.RIGHT:
                        PlayerMoveRight();
                        break;
                }

                m_MoveQueue.RemoveAt(0);
            }
        }

//        if(Input.GetKeyDown(KeyCode.Q))
//        {
//            GetComponent<Animator>().SetTrigger("Deflect");
//        }
    }

    void QueuePlayerMove(MoveDirections move)
    {
        if (m_MoveQueue.Count < m_MaxSizeOfMoveQueue)
        {
            m_MoveQueue.Add(move);
        }
    }

    void PlayerInputSwipeLeft()
    {
        QueuePlayerMove(MoveDirections.LEFT);
    }

    void PlayerMoveLeft()
    {
        if(buildingManager.IsValidMove(GameVariables.instance.PlayerManager.CurrentPlayer.BoardPosition + new Vector2(-1.0f, 0.0f)))
        {
            if(IsIdle())
            {
                GetComponent<Animator>().SetTrigger("Left");
                NotificationCenter.DefaultCenter().PostNotification(null, "ValidInput");
            }
        }
    }

    void PlayerInputSwipeRight()
    {
        QueuePlayerMove(MoveDirections.RIGHT);
    }

    void PlayerMoveRight()
    {
        if(buildingManager.IsValidMove(GameVariables.instance.PlayerManager.CurrentPlayer.BoardPosition + new Vector2(1.0f, 0.0f)))
        {
            if(IsIdle())
            {
                GetComponent<Animator>().SetTrigger("Right");
                NotificationCenter.DefaultCenter().PostNotification(null, "ValidInput");
            }
        }
    }

    void PlayerInputSwipeUp()
    {
        QueuePlayerMove(MoveDirections.UP);
    }

    void PlayerMoveUp()
    {
        if(buildingManager.IsValidMove(GameVariables.instance.PlayerManager.CurrentPlayer.BoardPosition + new Vector2(0.0f, 1.0f)))
        {
            if(IsIdle())
            {
                GetComponent<Animator>().SetTrigger("Up");
                NotificationCenter.DefaultCenter().PostNotification(null, "ValidInput");
            }
        }
    }

    void PlayerInputSwipeDown()
    {
        QueuePlayerMove(MoveDirections.DOWN);
    }

    void PlayerMoveDown()
    {
        // Make sure the player stays on the screen
        if(buildingManager.IsValidMove(GameVariables.instance.PlayerManager.CurrentPlayer.BoardPosition + new Vector2(0.0f, -1.0f)))
        {
            if(IsIdle())
            {
                GetComponent<Animator>().SetTrigger("Down");
                NotificationCenter.DefaultCenter().PostNotification(null, "ValidInput");
            }
        }
    }

    void PlayerInputTap()
    {
        PlayerInputSwipeUp();
    }

    bool IsIdle()
    {
        if(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("IdleLeft") ||
           GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("IdleRight"))
        {
            return true;
        }

        return false;
    }

    bool IsStationary()
    {
        if (IsIdle () ||
            GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("DeflectLeft") ||
            GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("DeflectRight"))
        {
            return true;
        }

        return false;
    }

    public void ClimberMoveComplete()
    {
        ChangedPosition.Invoke();

        m_FootStepAudio.clip = m_FootstepClips[Random.Range(0, m_FootstepClips.Count)];
        m_FootStepAudio.Play();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        DroppedObject droppedObject = collider.GetComponent<DroppedObject> ();

        if (droppedObject != null)
        {
            if (droppedObject.Armed)
            {
                if (IsStationary() && droppedObject.Deflectable)
                {
                    NotificationCenter.DefaultCenter().PostNotification(gameObject, "PlayerDeflectedGrenade");
                    GetComponent<Animator>().SetTrigger("Deflect");
                    DualPistolasAchievementManager.GetInstance().IncrementProgress(DualPistolasAchievementManager.DualPistolasAchievementId.ITS_RAINING, 1);
                    DualPistolasAchievementManager.GetInstance().IncrementProgress(DualPistolasAchievementManager.DualPistolasAchievementId.ITS_POURING, 1);

                    int bounceDirection = 1;

                    if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("IdleRight") ||
                        GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("DeflectRight"))
                    {
                        bounceDirection = 0;
                    }
                      
                    droppedObject.Bounce(bounceDirection);
                    m_DeflectGrenadeAudio.Play();
                }
                else
                {
                    TriggerDeath();
                    droppedObject.Explode();
                }
            }
        }
        else if(collider.name == "Electricity")
        {
            TriggerElectrocution();
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.name == "GameCamera")
        {
            if(IsAlive == true)
            {
                IsAlive = false;
                TriggerDeath();
            }

            NotificationCenter.DefaultCenter().PostNotification(null, "LoseGame");
        }
    }

    public void TriggerDeath()
    {
#if UNITY_EDITOR
        if (GameVariables.instance.EasyWin)
            return;
#endif

        if (IsAlive)
        {
            NotificationCenter.DefaultCenter().PostNotification(gameObject, "EnablePlayerInput", false);
            IsAlive = false;
            GetComponent<Animator>().SetTrigger("Die");

            m_FallAudio.PlayDelayed(0.4f);
        }
    }

    void TriggerElectrocution()
    {
#if UNITY_EDITOR
        if (GameVariables.instance.EasyWin)
            return;
#endif

        if (IsAlive)
        {
            NotificationCenter.DefaultCenter().PostNotification(gameObject, "EnablePlayerInput", false);
            IsAlive = false;
            GetComponent<Animator>().SetTrigger("Electrocution");

            m_ElectrocuteAudio.Play();
            StartCoroutine("StopElectrocutionSound");
            m_FallAudio.PlayDelayed(0.8f);
        }
    }

    IEnumerator StopElectrocutionSound()
    {
        yield return new WaitForSeconds(0.6f);
        m_ElectrocuteAudio.Stop();
    }
}
