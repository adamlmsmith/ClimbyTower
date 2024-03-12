using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Game : MonoBehaviour
{

    private bool GameOver { get; set; }

    void Awake()
    {
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "StartNewGame");
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "StartRound");
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "LoseGame");
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "BeginGameplay");

        // Disable all the children in case someone left one on (The game script also needs to be executed first before all others)
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    void Start()
    {
		GameVariables.instance.SignInScreen.gameObject.SetActive(true);
        GameVariables.instance.PlayerInputArea.gameObject.SetActive(true);

        GameOver = false;

//        if (GameManager.Instance.Authenticating)
//        {
//            return;
//        }
//        //Beep();
//        
//        if (GameManager.Instance.Authenticated)
//        {
//            //Beep();
//            GameManager.Instance.SignOut();
//        }
//        else
//        {
//            GameManager.Instance.Authenticate();
//        }
    }

#if UNITY_ANDROID
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!GameOver)
            {
                if(!GameVariables.instance.ClimberSelectScreen.gameObject.activeSelf)
                {
                    if(GameVariables.instance.StartScreen.gameObject.activeSelf)
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#else
                       
                        Application.Quit();
#endif
                    }
                    else
                    {
                        if(GameVariables.instance.PauseScreen.gameObject.activeSelf == false)
                            GameVariables.instance.HUD.PauseButtonPressed();
                    }
                }
            }
        }
    }
#endif
    
	void StartNewGame()
    {
        GameVariables.instance.PlayerManager.NumberOfPlayers = 1;
        GameVariables.instance.BoardHolder.gameObject.SetActive(true);
        NotificationCenter.DefaultCenter().PostNotification(gameObject, "EnablePlayerInput", false);

        NotificationCenter.DefaultCenter().PostNotification(gameObject, "StartRound");
        //NotificationCenter.DefaultCenter().PostNotification(gameObject, "BeginGameplay");
        GameOver = false;
    }

    void BeginGameplay()
    {
        NotificationCenter.DefaultCenter().PostNotification(gameObject, "EnablePlayerInput", true);
    }

    void TimesUp()
    {
        NotificationCenter.DefaultCenter().PostNotification(null, "LoseRound");
    }

    void StartRound()
    {
		NotificationCenter.DefaultCenter().PostNotification(gameObject, "ResetTimer");
    }

    void LoseGame()
    {
        GameVariables.instance.TallyScreen.gameObject.SetActive(true);

        DualPistolasAchievementManager.GetInstance().IncrementProgress(DualPistolasAchievementManager.DualPistolasAchievementId.GAMER, 1);
        DualPistolasAchievementManager.GetInstance().IncrementProgress(DualPistolasAchievementManager.DualPistolasAchievementId.VIDIOT, 1);

        GameOver = true;
    }
}
