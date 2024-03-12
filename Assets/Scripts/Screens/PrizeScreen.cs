using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PrizeScreen : MonoBehaviour
{
    public Text PrizeText;
    public Text NewCostumeText;
    public Text TryAgainText;
    public Button BuyPrizeButton;
    public Text BuyPrizeButtonText;
    public Button PlayButton;
    public Button ReturnButton;
    public GameObject UnlockedClimberHolder;
    Climber m_UnlockedClimber;

    [Header("Audio")]
    public AudioClip m_ButtonSound;
    public AudioClip m_ReturnSound;
    public AudioClip m_PrizeOpenSound;
    public AudioSource m_ClawMachineAudio;
    public AudioClip m_ClawMachineServoLongClip;
    public AudioClip m_ClawMachineServoShortClip;
    public AudioClip m_BallWhooshClip;

    void OnEnable()
    {
        GameVariables.instance.HUD.gameObject.SetActive(false);

        PrizeText.gameObject.SetActive(false);
        BuyPrizeButton.gameObject.SetActive(true);
        BuyPrizeButtonText.text = GameVariables.instance.CoinsPerPrize.ToString() + " Coins";

        PlayButton.gameObject.SetActive(false);
        ReturnButton.gameObject.SetActive(true);
        NewCostumeText.gameObject.SetActive(false);
        TryAgainText.gameObject.SetActive(false);
    }

    #if UNITY_ANDROID
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(ReturnButton.gameObject.activeSelf)
                ReturnClicked();
            else if(PlayButton.gameObject.activeSelf)
                PlayClicked();
        }
    }
    #endif

    public void BuyPrizeClicked()
    {
        GameVariables.instance.PlayerManager.CurrentPlayer.Coins -= GameVariables.instance.CoinsPerPrize;

        BuyPrizeButton.gameObject.SetActive(false);
        ReturnButton.gameObject.SetActive(false);

        GetComponent<Animator>().SetTrigger("OpenPrize");

        SoundManager.instance.PlaySingle(m_ButtonSound);

        m_ClawMachineAudio.clip = m_ClawMachineServoLongClip;
        m_ClawMachineAudio.Play();
    }

    public void PrizeExplosion()
    {
        Climber climberToUnlock = GameVariables.instance.ClimberManager.PickClimberForPrize();

        PrizeText.gameObject.SetActive(true);

        if (climberToUnlock != null)
        {
            ShowUnlockedClimber(climberToUnlock);
            PrizeText.text = climberToUnlock.ClimberDisplayName;
        }
        else
        {
            PrizeText.text = "All Climbers Unlocked";
        }

        ReturnButton.gameObject.SetActive(true);
        PlayButton.gameObject.SetActive(true);
        SoundManager.instance.PlaySingle(m_PrizeOpenSound);

        if (climberToUnlock.Unlocked == false)
        {
            NewCostumeText.gameObject.SetActive(true);
            GameVariables.instance.ClimberManager.UnlockClimber(climberToUnlock.ClimberName);
        
            DualPistolasAchievementManager.GetInstance().IncrementProgress(DualPistolasAchievementManager.DualPistolasAchievementId.WARDROBE_BEGINNER, 1);
            DualPistolasAchievementManager.GetInstance().IncrementProgress(DualPistolasAchievementManager.DualPistolasAchievementId.WARDROBE_ADVANCED, 1);
        } 
        else
        {
            TryAgainText.gameObject.SetActive(true);
        }

        GameManager.instance.ReportAllProgress();
    }

    void ShowUnlockedClimber(Climber climberToUnlock)
    {
        m_UnlockedClimber = Instantiate(climberToUnlock) as Climber;
        
        m_UnlockedClimber.transform.SetParent(UnlockedClimberHolder.transform);
        m_UnlockedClimber.transform.localPosition = new Vector3(0.0f, 0.0f, -0.1f);
        
        if(m_UnlockedClimber.transform.Find("ClimberTorso") != null)
            m_UnlockedClimber.transform.Find("ClimberTorso").transform.localPosition = Vector3.zero;
        
        m_UnlockedClimber.transform.localScale = new Vector3(77.0f, 77.0f, 77.0f);
        
        m_UnlockedClimber.GetComponent<Animator>().applyRootMotion = false;
        m_UnlockedClimber.GetComponent<Animator>().Play("DemoClimbLeft");
        
        m_UnlockedClimber.GetComponent<Climber>().enabled = false;

        foreach (Transform child in m_UnlockedClimber.GetComponentsInChildren<Transform>(true))
        {
            child.gameObject.layer = LayerMask.NameToLayer("UI");
        }
    }

    public void ReturnClicked()
    {
        if (m_UnlockedClimber)
            Destroy(m_UnlockedClimber.gameObject);
        
        gameObject.SetActive(false);
        GameVariables.instance.TallyScreen.gameObject.SetActive(true);

        SoundManager.instance.PlaySingle(m_ReturnSound);
    }

    public void PlayClicked()
    {
        int unlockedClimberIndex = GameVariables.instance.ClimberManager.GetClimberIndexForName(m_UnlockedClimber.ClimberName);

        GameVariables.instance.ClimberManager.SelectedClimberIndex = unlockedClimberIndex;

        if (m_UnlockedClimber)
            Destroy(m_UnlockedClimber.gameObject);

        gameObject.SetActive(false);

        GameVariables.instance.LogoScreen.gameObject.SetActive(true);
        SoundManager.instance.PlaySingle(m_ButtonSound);
    }

    void DropClaw()
    {
        m_ClawMachineAudio.clip = m_ClawMachineServoShortClip;
        m_ClawMachineAudio.Play();
    }

    void PlayBallWhoosh()
    {
        m_ClawMachineAudio.clip = m_BallWhooshClip;
        m_ClawMachineAudio.Play();
    }

}