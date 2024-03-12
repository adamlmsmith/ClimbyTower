using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PauseScreen : MonoBehaviour 
{
    public Slider m_MusicVolumeSlider;
    public Slider m_EffectsVolumeSlider;

    public AudioClip m_IntroClip;

    public AudioClip m_ButtonSound;
    public AudioClip m_ReturnSound;

	void OnEnable()
	{
		Time.timeScale = 0.0f;
		NotificationCenter.DefaultCenter().PostNotification(null, "PauseTimer");

        m_MusicVolumeSlider.value = SoundManager.instance.MusicVolume;
        m_EffectsVolumeSlider.value = SoundManager.instance.EffectsVolume;

        transform.Find("DebugCanvas").gameObject.SetActive(Debug.isDebugBuild);

        GetComponent<Animator>().Update(0.0f);
        GetComponent<Animator>().SetBool("On", true);

        SoundManager.instance.PlaySingle(m_IntroClip);
	}

    void OnDisable()
    {
        Time.timeScale = 1.0f;
    }

#if UNITY_ANDROID
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResumeGamePressed();
        }
    }
#endif
    
	public void ResumeGamePressed()
    {
        GetComponent<Animator>().SetBool("On", false);
        PlayerPrefs.Save ();

        SoundManager.instance.PlaySingle(m_ButtonSound);
    }

	public void QuitGamePressed()
	{
        Time.timeScale = 1.0f;

        GetComponent<Animator>().SetBool("On", false);
        gameObject.SetActive(false);
        GameVariables.instance.LogoScreen.gameObject.SetActive(true);
        GameVariables.instance.LogoScreen.GetComponent<Animator>().Update(0.0f);

        SoundManager.instance.PlaySingle(m_ButtonSound);
	}

    public void MusicSliderValueChanged(float musicVolume)
    {
        SoundManager.instance.MusicVolume = musicVolume;
    }

    public void SFXSliderValueChanged(float effectsVolume)
    {
        SoundManager.instance.EffectsVolume = effectsVolume;
    }

    public void ClearPlayerPrefsPressed()
    {
        if (Debug.isDebugBuild)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            QuitGamePressed();
        }
    }

    public void GiveCoinsCheat()
    {
        if (Debug.isDebugBuild)
        {
            GameVariables.instance.PlayerManager.CurrentPlayer.Coins += 25;
        }
    }

    public void UnlockAllClimbersCheat()
    {
        if (Debug.isDebugBuild)
        {
            int numTries = 0;
            
            while(!GameVariables.instance.ClimberManager.AllClimbersUnlocked())
            {
                numTries++;
                
                Climber climberToUnlock = GameVariables.instance.ClimberManager.PickClimberForPrize();
                GameVariables.instance.ClimberManager.UnlockClimber(climberToUnlock.ClimberName);
            }
            
            //Debug.Log("Required prizes = " + numTries);
        }
    }

    public void ClearGameCenterCheat()
    {
        if (Debug.isDebugBuild)
        {
            DualPistolasAchievementManager.GetInstance().DebugClearAchievements();
        }
    }

    public void TestIncrementAchievement()
    {
        if (Debug.isDebugBuild)
        {
            DualPistolasAchievementManager.GetInstance().IncrementProgress(DualPistolasAchievementManager.DualPistolasAchievementId.TEST_INCREMENT, 1);
            GameManager.instance.ReportAllProgress();
        }
    }

    public void TestInstantAchievement()
    {
        if (Debug.isDebugBuild)
        {
            DualPistolasAchievementManager.GetInstance().IncrementProgress(DualPistolasAchievementManager.DualPistolasAchievementId.TEST_INSTANT, 1);
            GameManager.instance.ReportAllProgress();
        }
    }
}
