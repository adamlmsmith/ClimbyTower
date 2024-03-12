using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CreditsScreen : MonoBehaviour
{
    public Text m_VersionText;
    public Button m_ReturnButton;
    public AudioClip m_ButtonSound;
    public AudioClip m_ReturnSound;

    public Button m_SignInButton;
    public Text m_SignInButtonText;

    public GameObject m_GooglePlayItems;

    void OnEnable()
    {
        m_VersionText.text = "v" + CurrentBundleVersion.version;

#if UNITY_ANDROID
        m_GooglePlayItems.SetActive(true);
#elif UNITY_IOS
        m_GooglePlayItems.SetActive(false);
#endif
    }

    #if UNITY_ANDROID
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(m_ReturnButton.gameObject.activeSelf)
                ReturnClicked();
        }

        if (GameManager.instance.Authenticated)
        {
            m_SignInButtonText.text = "Sign Out";
            PlayerPrefs.SetInt("AutoSignIn", 1);
            PlayerPrefs.SetInt("DeniedSignIn", 0);
        } 
        else if(GameManager.instance.Authenticating)
        {
            m_SignInButtonText.text = "Signing In..";
        }
        else
        {
            m_SignInButtonText.text = "Sign In";
        }
    }
    #endif

    public void TwitterLinkClicked()
    {
        Application.OpenURL ("https://twitter.com/AdamLMSmith");    
    }

    public void SignInClicked()
    {
        if (GameManager.instance.Authenticated == false)
        {
            if (GameManager.instance.Authenticating == false)
                GameManager.instance.Authenticate();
        }
        else
        {
            GameManager.instance.SignOut();
            PlayerPrefs.SetInt("AutoSignIn", 0);
            PlayerPrefs.SetInt("DeniedSignIn", 1);
            PlayerPrefs.Save();
        }
    }

    public void ReturnClicked()
    {
        gameObject.SetActive(false);
        SoundManager.instance.PlaySingle(m_ReturnSound);
    }
}
