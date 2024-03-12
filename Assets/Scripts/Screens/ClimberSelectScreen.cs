using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ClimberSelectScreen : MonoBehaviour
{
    public GameObject ClimberSelectButtonPrefab;
    public ScrollRect ScrollView;
    public GameObject ContentPanel;
    public Text ClimbersUnlockedText;
    public Button PlayButton;
    public Button BuyButton;

    List<ClimberSelectButton> m_ClimberSelectButtons = new List<ClimberSelectButton>();
    int highlightedClimberIndex = 0;

    public AudioClip m_LeftRightArrowSound;
    public AudioClip m_ButtonSound;
    public AudioClip m_ReturnSound;

    void Awake()
    {
        for (int i = 0; i < GameVariables.instance.ClimberManager.GetClimberCount(); i++)
        {
            // Instantiate a new button
            ClimberSelectButton newButton = GameObject.Instantiate(ClimberSelectButtonPrefab).GetComponent<ClimberSelectButton>();

            newButton.transform.SetParent(ContentPanel.transform);
            newButton.AddClimber(i);

            m_ClimberSelectButtons.Add(newButton);
        }

        NotificationCenter.DefaultCenter().AddObserver(gameObject, k.MessageNames.CLIMBER_COSTUME_CLICKED);
    }

    void Start()
    {
        MoveListToIndex(GameVariables.instance.ClimberManager.SelectedClimberIndex);
    }

    public void Initialize()
    {
        for (int i = 0; i < m_ClimberSelectButtons.Count; i++)
        {
            // Don't animate the random climber dice
            m_ClimberSelectButtons[i].Initialize(i != 0);
        }

        // Subtract one from each count so we don't consider the Random climber as an unlockable climber.
        ClimbersUnlockedText.text = (GameVariables.instance.ClimberManager.GetUnlockedClimberCount() - 1) + "/" + (GameVariables.instance.ClimberManager.GetClimberCount() - 1);
        
        MoveListToIndex(GameVariables.instance.ClimberManager.SelectedClimberIndex);
    }

#if UNITY_ANDROID
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnClicked();
        }
    }
#endif

    void MoveListToIndex(int index)
    {
        ScrollView.horizontalNormalizedPosition = ((float)index / (float)(m_ClimberSelectButtons.Count - 1));
        HighlightClimber(index);
    }

    int FindHighlightedClimber()
    {
        float minDistance = 99999.99f;
        int returnClimberIndex = 0;

        for(int i = 0; i < m_ClimberSelectButtons.Count; i++)
        {
            float distance = Vector3.Distance(m_ClimberSelectButtons[i].transform.position, ScrollView.transform.position);
            
            if(distance < minDistance)
            {
                minDistance = distance;
                returnClimberIndex = i;
            }
        }

        return returnClimberIndex;
    }
    
    void HighlightClimber(int climberIndex)
    {
        // Reset all climbers to normal scale and color
        for(int i = 0; i < m_ClimberSelectButtons.Count; i++)
        {
            m_ClimberSelectButtons[i].SetHighlighted(false);
        }

        m_ClimberSelectButtons [climberIndex].SetHighlighted(true);

        if(m_ClimberSelectButtons[climberIndex].IsClimberUnlocked())
        {
            PlayButton.gameObject.SetActive(true);
            BuyButton.gameObject.SetActive(false);
        }
        else
        {
            PlayButton.gameObject.SetActive(false);
            BuyButton.gameObject.SetActive(true);
        }
    }

    public void ReturnClicked()
    {
        gameObject.SetActive(false);
        GameVariables.instance.StartScreen.gameObject.SetActive(true);
        GameVariables.instance.PlayerInputArea.gameObject.SetActive(true);

        SoundManager.instance.PlaySingle(m_ReturnSound);
    }

    public void SelectClicked()
    {
        gameObject.SetActive(false);

        if (GameVariables.instance.ClimberManager.SelectedClimberIndex != highlightedClimberIndex)
        {
            GameVariables.instance.LogoScreen.gameObject.SetActive(true);
        }
        else
        {
            GameVariables.instance.StartScreen.gameObject.SetActive(true);
        }

        GameVariables.instance.ClimberManager.SelectedClimberIndex = highlightedClimberIndex;
        GameVariables.instance.PlayerInputArea.gameObject.SetActive(true);

        SoundManager.instance.PlaySingle(m_ButtonSound);

        GameManager.instance.ReportAllProgress();
    }

    public void BuyClicked()
    {

    }

    public void LeftArrowClicked()
    {
        int startingClimberIndex = highlightedClimberIndex;
        
        do
        {
            highlightedClimberIndex--;
            
            if (highlightedClimberIndex < 1)
                highlightedClimberIndex = m_ClimberSelectButtons.Count - 1;
        } 
        while(highlightedClimberIndex != startingClimberIndex &&
            m_ClimberSelectButtons[highlightedClimberIndex].IsClimberUnlocked() == false);
        
        MoveListToIndex(highlightedClimberIndex);

        SoundManager.instance.PlaySingle(m_LeftRightArrowSound);
    }

    public void RightArrowClicked()
    {
        int startingClimberIndex = highlightedClimberIndex;

        do
        {
            highlightedClimberIndex++;

            if (highlightedClimberIndex >= m_ClimberSelectButtons.Count)
                highlightedClimberIndex = 1;
        } 
        while(highlightedClimberIndex != startingClimberIndex &&
            m_ClimberSelectButtons[highlightedClimberIndex].IsClimberUnlocked() == false);

        MoveListToIndex(highlightedClimberIndex);

        SoundManager.instance.PlaySingle(m_LeftRightArrowSound);
    }

    public void ScrollViewChanged()
    {
        highlightedClimberIndex = FindHighlightedClimber();
        HighlightClimber(highlightedClimberIndex);
    }

    void ClimberCostumeClicked(Notification message)
    {
        if(PlayButton.IsActive())
            SelectClicked();
    }                                
}