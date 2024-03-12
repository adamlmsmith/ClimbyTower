using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
    public Text m_NextQuestText;
    public AudioClip m_ButtonSound;

    void Awake()
    {
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "ValidInput");
    }

    void OnEnable()
    {
        GetComponent<Animator>().SetBool("On", true);

        m_NextQuestText.text = GameVariables.instance.QuestManager.GetQuestString();

        SoundManager.instance.GameEffectsVolume = 0.0f;
    }

    public void ClimberSelectButtonClicked()
    {
        GameVariables.instance.ClimberSelectScreen.gameObject.SetActive(true);
        GameVariables.instance.ClimberSelectScreen.Initialize();
        GameVariables.instance.LogoScreen.gameObject.SetActive(false);
        gameObject.SetActive(false);
        GameVariables.instance.PlayerInputArea.gameObject.SetActive(false);

        SoundManager.instance.PlaySingle(m_ButtonSound);
    }

    void ValidInput()
    {
        GetComponent<Animator>().SetBool("On", false);
        GameVariables.instance.HUD.gameObject.SetActive(true);
    }
}