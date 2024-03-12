using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class RoundStats
{
    public virtual int Score { get; set; }
    public virtual bool WonRound { get; set; }
}

public class Player
{
    /// <summary>
    /// Player Score
    /// </summary>
    int m_Score = 0;


    /// <summary>
    /// Current Round Number
    /// </summary>
    int m_RoundNumber = 0;

    /// <summary>
    /// ID for the player 0 - 3
    /// </summary>
    int m_PlayerID = -1;


    /// <summary>
    /// collection of round stats based on the number of round set in the game var
    /// </summary>
    RoundStats[] m_RoundStats = null;

    Vector2 m_StartingClimberPos = new Vector2(2, 4);
    Vector2 m_BoardPosition;
    Vector2 m_HighestBoardPosition;

    public int PlayerID { get { return m_PlayerID; } set { m_PlayerID = value; } }
    public int RoundNumber { get { return m_RoundNumber; } set { m_RoundNumber = value; } }
    public Vector2 BoardPosition 
    { 
        get { return m_BoardPosition; } 
        set
        { 
            m_BoardPosition = value; 
            if(m_BoardPosition.y > HighestBoardPosition.y)
            {
                NotificationCenter.DefaultCenter().PostNotification(null, "PlayerClimbedFloor");
                GameManager.instance.IncrementEvent(GPGSIds.event_floor_climbed, 1);
                DualPistolasAchievementManager.GetInstance().IncrementProgress(DualPistolasAchievementManager.DualPistolasAchievementId.HUNDO_CLUB, 1);
                DualPistolasAchievementManager.GetInstance().IncrementProgress(DualPistolasAchievementManager.DualPistolasAchievementId.IM_FLOORED, 1);
                HighestBoardPosition = value;
                Score++;
            }
        }
    }

    public Vector2 HighestBoardPosition
    { 
        get { return m_HighestBoardPosition; } 
        set
        {
            m_HighestBoardPosition = value;
        }
    }

    public int Coins
    {
        get
        {
            return GameManager.instance.Progress.Coins;
        }
        set
        {
            GameManager.instance.Progress.Coins = value;
            NotificationCenter.DefaultCenter().PostNotification(null, "PlayerCoinsChanged");
        }
    }

//    public int FloorsClimbed
//    {
//        get
//        {
//            return m_FloorsClimbed;
//        }
//        set
//        {
//            m_FloorsClimbed = value;
//        }
//    }

    public int Score
    {
        get
        {
            return m_Score;
        }
        set
        {
            m_Score = value;
			NotificationCenter.DefaultCenter().PostNotification(null, "PlayerScoreChanged");

            TopScore = Mathf.Max(TopScore, m_Score);
        }
    }

    public int TopScore
    {
        get { return GameManager.instance.Progress.TopScore; }
        set
        { 
            GameManager.instance.Progress.TopScore = value; 
        }
    }

    public RoundStats CurrentRoundStats { get { return m_RoundStats[RoundNumber]; } }
    public RoundStats[] RoundStats { get { return m_RoundStats; } }
    public bool LostTheGame { get; set; }
    

    public virtual void Initialize()
    {
        m_RoundStats = new RoundStats[GameVariables.instance.MaxNumRounds];

        for (int i = 0; i < RoundStats.Length; i++)
        {
            m_RoundStats[i] = new RoundStats();
        }

        //TopScore = GameManager.instance.Progress.TopScore;
		m_Score = 0;
		RoundNumber = 0;
        //Coins = GameManager.instance.Progress.Coins;
        //FloorsClimbed = 0;

        m_BoardPosition = m_StartingClimberPos;
        m_HighestBoardPosition = m_BoardPosition;
        PlayerPrefs.Save();
    }

    public bool DidWinRound(int _roundNumber)
    { 
        return((_roundNumber < 0 || _roundNumber >= m_RoundStats.Length) ? false : m_RoundStats[_roundNumber].WonRound);
    }
}
