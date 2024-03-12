using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Text m_ScoreText;
    public Text m_CoinsText;

    void Awake()
    {
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "PlayerCoinsChanged");
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "PlayerScoreChanged");
    }

    void OnEnable()
    {
        NotificationCenter.DefaultCenter().PostNotification(null, "PlayerCoinsChanged");
    }

    void PlayerCoinsChanged()
    {
        m_CoinsText.text = "Coins: " + GameVariables.instance.PlayerManager.CurrentPlayer.Coins;
    }

    void PlayerScoreChanged()
    {
        m_ScoreText.text = "Score: " + GameVariables.instance.PlayerManager.CurrentPlayer.Score.ToString();
    }

    public void PauseButtonPressed()
    {
        GameVariables.instance.PauseScreen.gameObject.SetActive(true);
    }
}
