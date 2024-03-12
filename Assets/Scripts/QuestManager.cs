using UnityEngine;
using System.Collections;
using System;

public class QuestManager : MonoBehaviour
{
    public enum Quests { CLIMB_FLOORS, DEFLECT_GRENADES, TRIGGER_MINES, CLIMB_UNDER_POWERLINES };

    public bool ActiveQuest
    {
        get { return GameManager.instance.Progress.ActiveQuest; }
        set
        {
            GameManager.instance.Progress.ActiveQuest = value;
        }
    }

    public Quests CurrentQuest
    { 
        get { return (Quests)GameManager.instance.Progress.CurrentQuest; }
        set
        {
            GameManager.instance.Progress.CurrentQuest = (int)value;
        }
    }

    public int CurrentQuestValue
    {
        get { return GameManager.instance.Progress.CurrentQuestValue; }
        set
        {
            GameManager.instance.Progress.CurrentQuestValue = value;
        }
    }

    public int CurrentQuestGoal { get; set; }
    public string CurrentQuestDescription { get; set; }

    //DateTime m_DateTimeOfLastQuest;
    //DateTime m_DateTimeOfNextQuest;
 
    DateTime DateTimeOfLastQuest
    {
        get { return GameManager.instance.Progress.DateTimeOfLastQuest; }
        set 
        { 
            GameManager.instance.Progress.DateTimeOfLastQuest = value;
        }
    }

    DateTime DateTimeOfNextQuest
    {
        get { return GameManager.instance.Progress.DateTimeOfNextQuest; }
        set 
        { 
            GameManager.instance.Progress.DateTimeOfNextQuest = value;
        }
    }

    void Awake()
    {
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "PlayerClimbedFloor");
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "PlayerDeflectedGrenade");
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "PlayerTriggeredMine");
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "PlayerClimbedUnderPowerline");
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "StartNewGame");
    }

    void Start()
    {
        Initialize();
    }

    void GetNewQuest()
    {
        int randomQuestNum = UnityEngine.Random.Range(0, Quests.GetNames(typeof(Quests)).Length);

        UpdateQuestInformation(randomQuestNum);

        ResetQuestTimes();

        CurrentQuestValue = 0;
        ActiveQuest = true;
        GameManager.instance.ReportAllProgress();
    }

    void UpdateQuestInformation(int questNum)
    {
        switch (questNum)
        {
            case 0:
                CurrentQuest = Quests.CLIMB_FLOORS;
                CurrentQuestDescription = "Floors";
                CurrentQuestGoal = 600;
                break;
                
            case 1:
                CurrentQuest = Quests.DEFLECT_GRENADES;
                CurrentQuestDescription = "Grenades Deflected";
                CurrentQuestGoal = 50;
                break;

            case 2:
                CurrentQuest = Quests.TRIGGER_MINES;
                CurrentQuestDescription = "Mines Triggered";
                CurrentQuestGoal = 50;
                break;

            case 3:
                CurrentQuest = Quests.CLIMB_UNDER_POWERLINES;
                CurrentQuestDescription = "Powerlines";
                CurrentQuestGoal = 50;
                break;
                
            default:
                Debug.LogError("Error - Undefined Quest Data");
                break;
        }
    }

    public bool IsActiveQuestCompleted()
    {
        return(CurrentQuestValue >= CurrentQuestGoal);
    }

    public void ClaimQuest()
    {
        ActiveQuest = false;
        CurrentQuestValue = 0;
        ResetQuestTimes();
    }

    void ResetQuestTimes()
    {
        DateTimeOfLastQuest = DateTime.UtcNow;
        DateTimeOfNextQuest = DateTime.UtcNow.AddDays(1);
    }

    bool IsTimeForNewQuest()
    {
        return(DateTimeOfNextQuest.CompareTo(DateTime.UtcNow) < 0);
    }

    public void CheckForNewQuest()
    {
        if (IsTimeForNewQuest())
        {
            GetNewQuest();
        }
    }

    public string GetQuestString()
    {
        string returnString;

        if(ActiveQuest)
        {
            returnString = CurrentQuestValue + "/" + CurrentQuestGoal + " " + CurrentQuestDescription;           
        }
        else
        {
            TimeSpan timeUntilNextQuest = DateTimeOfNextQuest.Subtract(DateTime.UtcNow);

            if(timeUntilNextQuest.Hours > 0)
            {
                returnString = "Next Quest In " + timeUntilNextQuest.Hours + "H " + timeUntilNextQuest.Minutes + "M";
            }
            else
            {
                returnString = "Next Quest In " + timeUntilNextQuest.Minutes + "M";
            }
        }

        return returnString;
    }

    void Initialize()
    {
        ActiveQuest = GameManager.instance.Progress.ActiveQuest;
        CurrentQuest = (Quests)GameManager.instance.Progress.CurrentQuest;
        CurrentQuestValue = GameManager.instance.Progress.CurrentQuestValue;

        UpdateQuestInformation((int)CurrentQuest);

        if (GameManager.instance.Progress.DateTimeOfLastQuest != DateTime.MinValue)
        {
            //long temp = Convert.ToInt64(GameManager.instance.Progress.DateTimeOfLastQuest);
            
            //DateTimeOfLastQuest = DateTime.FromBinary(temp);
        }
        else
        {
            GetNewQuest();
        }
        
        DateTimeOfNextQuest = DateTimeOfLastQuest.AddDays(1);
    }

    void PlayerClimbedFloor()
    {
        if (ActiveQuest == true && CurrentQuest == Quests.CLIMB_FLOORS)
        {
            CurrentQuestValue = Mathf.Clamp(CurrentQuestValue + 1, 0, CurrentQuestGoal);
        }
    }

    void PlayerDeflectedGrenade()
    {
        if (ActiveQuest == true && CurrentQuest == Quests.DEFLECT_GRENADES)
        {
            CurrentQuestValue = Mathf.Clamp(CurrentQuestValue + 1, 0, CurrentQuestGoal);
        }
    }

    void PlayerTriggeredMine()
    {
        if (ActiveQuest == true && CurrentQuest == Quests.TRIGGER_MINES)
        {
            CurrentQuestValue = Mathf.Clamp(CurrentQuestValue + 1, 0, CurrentQuestGoal);
        }
    }

    void PlayerClimbedUnderPowerline()
    {
        if (ActiveQuest == true && CurrentQuest == Quests.CLIMB_UNDER_POWERLINES)
        {
            CurrentQuestValue = Mathf.Clamp(CurrentQuestValue + 1, 0, CurrentQuestGoal);
        }
    }

    void StartNewGame()
    {
        CheckForNewQuest();
    }
}
