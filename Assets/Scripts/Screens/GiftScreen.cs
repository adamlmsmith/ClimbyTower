using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GiftScreen : MonoBehaviour
{
    [SerializeField]
    int m_MinCoins = 40;

    [SerializeField]
    int m_MaxCoins = 120;

    [SerializeField]
    Text m_GiftText;

    [SerializeField]
    Button m_OpenGiftButton;

    [SerializeField]
    Button m_ReturnButton;

    [SerializeField]
    GameObject m_CoinExplosion;

    [Header("Audio")]
    public AudioClip m_DrumRollClip;
    public AudioSource m_GiftScreenAudio;

    void OnEnable()
    {
        m_ReturnButton.gameObject.SetActive(false);
        m_OpenGiftButton.gameObject.SetActive(true);
        m_GiftText.gameObject.SetActive(false);
        m_CoinExplosion.SetActive(false);
    }

    #if UNITY_ANDROID
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(m_ReturnButton.gameObject.activeSelf)
                ReturnClicked();
        }
    }
    #endif

    public void OpenGiftClicked()
    {
        GetComponent<Animator>().SetTrigger("OpenGift");

        m_OpenGiftButton.gameObject.SetActive(false);

        m_GiftScreenAudio.clip = m_DrumRollClip;
        m_GiftScreenAudio.Play();
    }

    public void ReturnClicked()
    {
        gameObject.SetActive(false);
        GameVariables.instance.TallyScreen.gameObject.SetActive(true);
    }

    public void GiftExplosion()
    {
        int coinsToGive = Random.Range(m_MinCoins, m_MaxCoins + 1);

        if (GameVariables.instance.GiftManager.FirstGift)
        {
            coinsToGive = GameVariables.instance.CoinsPerPrize;
            GameVariables.instance.GiftManager.FirstGift = false;
        }



        GameVariables.instance.PlayerManager.CurrentPlayer.Coins += coinsToGive;
        m_GiftText.text = coinsToGive + " Coins";

        m_CoinExplosion.SetActive(true);
        m_GiftText.gameObject.SetActive(true);

        m_ReturnButton.gameObject.SetActive(true);

        DualPistolasAchievementManager.GetInstance().IncrementProgress(DualPistolasAchievementManager.DualPistolasAchievementId.COIN_EXPLOSION, 1);
        DualPistolasAchievementManager.GetInstance().IncrementProgress(DualPistolasAchievementManager.DualPistolasAchievementId.RE_GIFTER, 1);
        GameManager.instance.ReportAllProgress();
    }
}