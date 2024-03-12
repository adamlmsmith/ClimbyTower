using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SignInScreen : MonoBehaviour
{
    public AudioClip m_ButtonSound;
    public Text m_SignInButtonText;

    void Start()
    {
#if UNITY_IOS
        PlayerPrefs.SetInt("AutoSignIn", 1);
#endif

        if (PlayerPrefs.GetInt("DeniedSignIn") == 1)
        {
            ExitScreen();
        } else
        {
            if (PlayerPrefs.GetInt("AutoSignIn") == 1)
            {
                GameManager.instance.Authenticate();
                ExitScreen();
            }
        }
    }


    void Update()
    {
        #if UNITY_ANDROID
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            NotNowClicked();
        }
        #endif

        if (GameManager.instance.Authenticated == false)
        {
            if(GameManager.instance.Authenticating)
                m_SignInButtonText.text = "Signing In..";
            else
                m_SignInButtonText.text = "Sign In";
        }
        else
        {
            m_SignInButtonText.text = "Signed In";
            PlayerPrefs.SetInt("AutoSignIn", 1);
            ExitScreen();
        }
    }


    public void SignInClicked()
    {
        if (GameManager.instance.Authenticated == false)
        {
            if (GameManager.instance.Authenticating == false)
                GameManager.instance.Authenticate();
        }
    }

    public void NotNowClicked()
    {
        PlayerPrefs.SetInt("AutoSignIn", 0);
        PlayerPrefs.SetInt("DeniedSignIn", 1);
        PlayerPrefs.Save();
        ExitScreen();
    }

    void ExitScreen()
    {
        GameVariables.instance.LogoScreen.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}