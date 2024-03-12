using UnityEngine;
using System.Collections;
using System;

public class GiftManager : MonoBehaviour
{   
    public DateTime DateTimeOfLastGift
    {
        get { return GameManager.instance.Progress.DateTimeOfLastGift; }
        set
        {
            GameManager.instance.Progress.DateTimeOfLastGift = value;
        }
    }
    
    public DateTime DateTimeOfNextGift { get; set; }
    public bool FirstGift { get; set; }

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        // Check if this is the first time the game is being played
        if (GameManager.instance.Progress.DateTimeOfLastGift != DateTime.MinValue)
        {
            DateTimeOfNextGift = DateTimeOfLastGift.AddMinutes(GameVariables.instance.MinutesBetweenGifts);
            FirstGift = false;
        }
        else
        {
            GiveGiftCheat();
            FirstGift = true;
        }
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GiveGiftCheat();
        }
    }
#endif

    public void GiveGiftCheat()
    {
        GameVariables.instance.GiftManager.DateTimeOfNextGift = DateTime.UtcNow;
        GameVariables.instance.GiftManager.DateTimeOfLastGift = GameVariables.instance.GiftManager.DateTimeOfNextGift.AddMinutes(-GameVariables.instance.MinutesBetweenGifts);
    }

    public bool GiftAvailable()
    {
        return(GameVariables.instance.GiftManager.DateTimeOfNextGift.CompareTo(DateTime.UtcNow) < 0);
    }

    public void ClaimGift()
    {
        GameVariables.instance.GiftManager.DateTimeOfLastGift = DateTime.UtcNow;
        GameVariables.instance.GiftManager.DateTimeOfNextGift = DateTime.UtcNow.AddMinutes(GameVariables.instance.MinutesBetweenGifts);
    }

    public string GetTimeOfNextGiftString()
    {
        TimeSpan timeUntilNextGift = GameVariables.instance.GiftManager.DateTimeOfNextGift.Subtract(DateTime.UtcNow);
        string returnString = "Free Gift In " + timeUntilNextGift.Hours + "H " + timeUntilNextGift.Minutes + "M";

        return returnString;
    }
}
