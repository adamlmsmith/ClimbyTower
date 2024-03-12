using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.Advertisements;

public class TallyScreen : MonoBehaviour
{
    public GameObject m_EarnCoinsButton;
    public GameObject m_FreeGiftButton;
    public GameObject m_RateUsButton;
    public GameObject m_WinAPrizeButton;

    public GameObject m_CoinsToGo;
    public GameObject m_CurrentQuest;
    public GameObject m_FreeGiftInTime;
    public GameObject m_GameTip;

    public GameObject m_PreviewThreeCostumes;
    public GameObject m_BuyPreviewedCostume;
    public GameObject m_BuyPiggyBank;

    public Text m_ScoreText;
    public Text m_TopScoreText;

    const int MaxNumObjectsToShowOnScreen = 3;
    const float ChanceOfShowingPurchasePromoObject = 0.3f;
    const float m_ChanceOfShowingGameTip = 0.33f;

    public List<Color> m_ButtonColorList = new List<Color>();

    public AudioClip m_ButtonSound;
    public AudioClip m_LineIntroSound;

    public string[] m_GameTipStrings = new string[]
    { 
        "Tip: Stay still to deflect falling objects", 
        "Tip: Falling objects only hurt when climbing",
        "Tip: Power Lines glow blue before electrifying",
        "Tip: Tapping the screen also climbs up",
        "Tip: You can swipe in all four directions, or just tap to go up faster",
        "Tip: Collect coins to use the prize machine",
        "Tip: Return to the game every 6 hours to win a free gift",
        "Tip: Watch a quick video ad to earn a free gift instantly",
        "Tip: Complete a quest to earn a gift",
        "Tip: Gifts are a quick way to earn coins",
        "Tip: Proximity mines detonate after 3 seconds",
        "Tip: Don't touch the bottom of the screen or you'll fall",
        "Tip: Sometimes going slow and waiting for an opening is better"
    };


    void OnEnable()
    {     
        List<GameObject> showableObjects = new List<GameObject>();
        List<GameObject> objectsToShow = new List<GameObject>();

        DisableAllObjects();
        int numObjectsToShow = 0;
        bool prizeAvailable = false;
        bool questClaimable = false;
        bool giftAvailable = false;

        //bool hasPiggyBank = false;
        bool isUsingPreviewCostume = false;
        int previewCostumeRoundsLeft = 0;

        if (isUsingPreviewCostume && previewCostumeRoundsLeft == 0)
        {
            showableObjects.Add(m_BuyPreviewedCostume);
            numObjectsToShow = 1;
        } 
        else
        {
            if (GameVariables.instance.PlayerManager.CurrentPlayer.Coins >= GameVariables.instance.CoinsPerPrize)
            {
                objectsToShow.Add(m_WinAPrizeButton);
                prizeAvailable = true;
                numObjectsToShow++;
            }

            if(GameVariables.instance.QuestManager.ActiveQuest && GameVariables.instance.QuestManager.IsActiveQuestCompleted())
            {
                m_CurrentQuest.GetComponent<Button>().interactable = true;
                m_CurrentQuest.transform.Find("Button").GetComponent<Button>().interactable = true;
                m_CurrentQuest.transform.Find("Text").GetComponent<Text>().text = GameVariables.instance.QuestManager.GetQuestString();
                objectsToShow.Add(m_CurrentQuest);
                questClaimable = true;
                numObjectsToShow++;
            }

            if (GameVariables.instance.GiftManager.GiftAvailable())
            {
                objectsToShow.Add(m_FreeGiftButton);
                giftAvailable = true;
                numObjectsToShow++;
            }
            

            //if (showableObjects.Count == 0)
            {
//                if((GameVariables.instance.ClimberManager.GetLockedClimberCount() > 0) &&
//                   (UnityEngine.Random.Range(0.0f, 1.0f) < ChanceOfShowingPurchasePromoObject))
//                {
//                    showableObjects.Add(m_PreviewThreeCostumes);
//                    showableObjects.Add(m_BuyPreviewedCostume);
//
//                    if (hasPiggyBank == false)
//                        showableObjects.Add(m_BuyPiggyBank);
//
//                    numObjectsToShow = 1;
//                }
//                else
                {
                    numObjectsToShow = UnityEngine.Random.Range(0, MaxNumObjectsToShowOnScreen + 1);
                    numObjectsToShow -= objectsToShow.Count;

                    if (numObjectsToShow > 0)
                    {
                        if(!giftAvailable)
                        {
                            // Free Gift In Time
                            m_FreeGiftInTime.transform.Find("Text").GetComponent<Text>().text = GameVariables.instance.GiftManager.GetTimeOfNextGiftString();
                            showableObjects.Add(m_FreeGiftInTime);
                        }

                        if(!prizeAvailable)
                        {
                            // Coins To Go
                            m_CoinsToGo.transform.Find("Text").GetComponent<Text>().text = (GameVariables.instance.CoinsPerPrize - GameVariables.instance.PlayerManager.CurrentPlayer.Coins) + " Coins To Go";
                            showableObjects.Add(m_CoinsToGo);
                        }

                        if(!questClaimable)
                        {
                            // Current Quest
                            m_CurrentQuest.transform.Find("Text").GetComponent<Text>().text = GameVariables.instance.QuestManager.GetQuestString();
                            showableObjects.Add(m_CurrentQuest);
                        }


                        if(GameVariables.instance.QuestManager.ActiveQuest && GameVariables.instance.QuestManager.IsActiveQuestCompleted())
                        {
                            m_CurrentQuest.GetComponent<Button>().interactable = true;
                            m_CurrentQuest.transform.Find("Button").GetComponent<Button>().interactable = true;
                        }
                        else
                        {
                            m_CurrentQuest.GetComponent<Button>().interactable = false;
                            m_CurrentQuest.transform.Find("Button").GetComponent<Button>().interactable = false;
                        }                        

                        if(AdManager.instance.IsAdReady("rewardedVideo"))
                            showableObjects.Add(m_EarnCoinsButton);

                        showableObjects.Add(m_RateUsButton);
                    }
                }
            }
        }

        for (int i = 0; i < numObjectsToShow; i++)
        {
            if (showableObjects.Count > 0)
            {
                int randomNum = UnityEngine.Random.Range(0, showableObjects.Count);
                objectsToShow.Add(showableObjects [randomNum]);
                showableObjects.RemoveAt(randomNum);
            }
        }

        if (objectsToShow.Count == 0 || UnityEngine.Random.Range(0.0f, 1.0f) < m_ChanceOfShowingGameTip)
        {
            objectsToShow.Add(m_GameTip);

            int randomTipNumber = UnityEngine.Random.Range(0, m_GameTipStrings.Length);
            m_GameTip.transform.Find("Text").GetComponent<Text>().text = m_GameTipStrings[randomTipNumber];
        }

        for(int i = 0; i < objectsToShow.Count; i++)
        {         
            objectsToShow[i].SetActive(true);

            if(objectsToShow[i] == m_PreviewThreeCostumes ||
               objectsToShow[i] == m_BuyPiggyBank ||
               objectsToShow[i] == m_BuyPreviewedCostume ||
               objectsToShow[i] == m_GameTip)
            {
                objectsToShow[i].GetComponent<Image>().color = m_ButtonColorList[3]; // TODO improve this
            }
            else
            {
                objectsToShow[i].GetComponent<Image>().color = m_ButtonColorList[i];
            }

            objectsToShow[i].transform.localScale = Vector3.one;
            iTween.ScaleFrom(objectsToShow[i].gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.2f, "delay", i * 0.1f, "onstart", "PlayLineIntroSound", "onstarttarget", gameObject));
            objectsToShow[i].transform.SetSiblingIndex(i);
        }

        if(GameVariables.instance.PlayerManager.CurrentPlayer.Score >= 25)
            DualPistolasAchievementManager.GetInstance().IncrementProgress(DualPistolasAchievementManager.DualPistolasAchievementId.GOOD_SCORE, 1);

        if(GameVariables.instance.PlayerManager.CurrentPlayer.Score >= 100)
            DualPistolasAchievementManager.GetInstance().IncrementProgress(DualPistolasAchievementManager.DualPistolasAchievementId.GREAT_SCORE, 1);

        if(GameVariables.instance.PlayerManager.CurrentPlayer.Score >= 250)
            DualPistolasAchievementManager.GetInstance().IncrementProgress(DualPistolasAchievementManager.DualPistolasAchievementId.AWESOME_SCORE, 1);

        PlayerPrefs.Save();

        Time.timeScale = 0.3f;

        GameVariables.instance.HUD.gameObject.SetActive(false);

        GetComponent<Animator>().Update(0.0f);
        GetComponent<Animator>().SetBool("On", true);

        m_ScoreText.text = "Score: " + GameVariables.instance.PlayerManager.CurrentPlayer.Score;
        m_TopScoreText.text = "Top: " + GameVariables.instance.PlayerManager.CurrentPlayer.TopScore;

        GameManager.instance.ReportAllProgress();

        SoundManager.instance.GameEffectsVolume = -80.0f;
    }

    #if UNITY_ANDROID
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameVariables.instance.CreditsScreen.gameObject.activeSelf == false)
                PlayGameClicked();
        }
    }
    #endif

    void PlayLineIntroSound()
    {
        SoundManager.instance.PlaySingle(m_LineIntroSound);
    }

    void DisableAllObjects()
    {
        // TODO rewrite this function
        m_EarnCoinsButton.SetActive(false);
        m_FreeGiftButton.SetActive(false);
        m_RateUsButton.SetActive(false);
        
        m_CoinsToGo.SetActive(false);
        m_CurrentQuest.SetActive(false);
        m_WinAPrizeButton.SetActive(false);
        m_FreeGiftInTime.SetActive(false);
        m_GameTip.SetActive(false);
        
        m_PreviewThreeCostumes.SetActive(false);
        m_BuyPreviewedCostume.SetActive(false);
        m_BuyPiggyBank.SetActive(false);
    }

    public void EarnCoinsClicked()
    {
        SoundManager.instance.PlaySingle(m_ButtonSound);
        AdManager.instance.ShowAd("rewardedVideo", ShowAdCallback);
    }

    void ShowAdCallback(ShowResult showResult)
    {
        switch (showResult)
        {
            case ShowResult.Finished:
                GameVariables.instance.GiftScreen.gameObject.SetActive(true);
                ExitTallyScreen();
                break;
            case ShowResult.Skipped:
                ReturnToStartScreen();
                break;
            case ShowResult.Failed:
                ReturnToStartScreen();
                break;
        }
    }

    public void FreeGiftClicked()
    {
        SoundManager.instance.PlaySingle(m_ButtonSound);
        GameVariables.instance.GiftManager.ClaimGift();
        GameVariables.instance.GiftScreen.gameObject.SetActive(true);
        ExitTallyScreen();
    }

    public void RateUsClicked()
    {
        SoundManager.instance.PlaySingle(m_ButtonSound);
        ReturnToStartScreen();

        if(Application.platform == RuntimePlatform.Android)
            //Application.OpenURL ("market://search?q=Climby Tower"); //Replace 'Unity Remote' by <Your Publisher name>
            Application.OpenURL ("market://details?id=com.DualPistolas.ClimbyTower");
        else if(Application.platform == RuntimePlatform.IPhonePlayer)
            Application.OpenURL("https://itunes.apple.com/us/app/climby-tower/id1064085458?ls=1&mt=8");
        else
            Application.OpenURL("http://www.dualpistolas.com");
    }

    public void SettingsClicked()
    {
        SoundManager.instance.PlaySingle(m_ButtonSound);
        GameVariables.instance.CreditsScreen.gameObject.SetActive(true);
    }
     
    public void ShareClicked()
    {
        SoundManager.instance.PlaySingle(m_ButtonSound);
        ReturnToStartScreen();
    }

    public void AchievementsClicked()
    {
        if (GameManager.instance.Authenticated == false)
        {
            GameManager.instance.Authenticate();

            if(GameManager.instance.Authenticated)
            {
                // TODO this never gets hit. Make it delay while authenticating
                StartCoroutine("VerifyAuthentication");
                SoundManager.instance.PlaySingle(m_ButtonSound);
                GameManager.instance.ShowAchievementsUI();
            }
        } 
        else
        {
            SoundManager.instance.PlaySingle(m_ButtonSound);
            GameManager.instance.ShowAchievementsUI();
        }
    }

    public void WinAPrizeClicked()
    {
        SoundManager.instance.PlaySingle(m_ButtonSound);
        GameVariables.instance.PrizeScreen.gameObject.SetActive(true);
        ExitTallyScreen();
    }

    public void CurrentQuestClicked()
    {
        SoundManager.instance.PlaySingle(m_ButtonSound);
        GameVariables.instance.QuestManager.ClaimQuest();
        GameVariables.instance.GiftScreen.gameObject.SetActive(true);
        ExitTallyScreen();
    }

    public void PlayGameClicked()
    {
        SoundManager.instance.PlaySingle(m_ButtonSound);
        ReturnToStartScreen();
    }

    public void LeaderboardsClicked()
    {
        if (GameManager.instance.Authenticated == false)
        {
            GameManager.instance.Authenticate();

            // TODO this never gets hit. Make it delay while authenticating
            if(GameManager.instance.Authenticated)
            {
                StartCoroutine("VerifyAuthentication");
                SoundManager.instance.PlaySingle(m_ButtonSound);
                GameManager.instance.ShowLeaderboardUI();
            }
        }
        else
        {
            SoundManager.instance.PlaySingle(m_ButtonSound);
            GameManager.instance.ShowLeaderboardUI();
        }
    }

    IEnumerator VerifyAuthentication()
    {
        yield return new WaitForSeconds(3.0f);

        if (GameManager.instance.Authenticated)
        {
            PlayerPrefs.SetInt("AutoSignIn", 1);
            PlayerPrefs.SetInt("DeniedSignIn", 0);
            PlayerPrefs.Save();
        }
    }

    void ReturnToStartScreen()
    {
        SoundManager.instance.PlaySingle(m_ButtonSound);
        GetComponent<Animator>().SetBool("On", true);
        GameVariables.instance.LogoScreen.gameObject.SetActive(true);
        ExitTallyScreen();
    }

    void ExitTallyScreen()
    {
        gameObject.SetActive(false);
        System.GC.Collect();
        Time.timeScale = 1.0f;
    }
}