using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class PlayerManager : MonoBehaviour
{
    //holds the number of players 
    int m_NumberOfPlayers = 1;

    //keeps track of the current player
    int m_CurrentPlayer = 0;

    //a list of all the players
    List<Player> m_Players = new List<Player>();
    
    public Player CurrentPlayer
    {
        get
        {
            // If the players haven't been created yet, at least create one.
            if (m_Players.Count == 0)
                NumberOfPlayers = 1;
            return m_Players[m_CurrentPlayer];
        }
    }

    public int NumberOfPlayers
    {
        get
        {
            return m_NumberOfPlayers;
        }
        set
        {
            m_NumberOfPlayers = value;
            CreatePlayers();
        }
    }
    
    void Awake()
    {
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "ResetScores");
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "AddScore");
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "StartNewGame");
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "WinRound");
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "LoseRound");
    }

    /// <summary>
    /// sets up each player
    /// </summary>
    void CreatePlayers()
    {
        m_Players.Clear();

        for (int i = 0; i < m_NumberOfPlayers; i++)
        {
            Player newPlayer = new Player();
            m_Players.Add(newPlayer);
            newPlayer.PlayerID = i;
            newPlayer.Initialize();
        }

        m_CurrentPlayer = 0;
    }

    public int RoundNumber
    {
        set
        {
            for (int i = 0; i < m_NumberOfPlayers; i++)
                m_Players[i].RoundNumber = value;
        }
    }

    /// <summary>
    /// gets the next player, first by increasing the round number, player number, 
    /// then checks if the player has lost the game or passed the max num rounds
    /// </summary>
    public void NextPlayer()
    {
        do
        {
            CurrentPlayer.RoundNumber++;
            m_CurrentPlayer++;

            if (m_CurrentPlayer >= m_NumberOfPlayers)
            {
                m_CurrentPlayer = 0;
            }
        }
        while (CurrentPlayer.LostTheGame == true && CurrentPlayer.RoundNumber < GameVariables.instance.MaxNumRounds);
    }


    public Player GetPlayer(int iPlayerID)
    {
        if (iPlayerID < 0 || iPlayerID >= m_NumberOfPlayers)
        {
            Debug.LogError(iPlayerID + " is an invalid player ID");
            return null;
        }

        return m_Players[iPlayerID];
    }
    
    void ResetScores()
    {
        for (int i = 0; i < m_NumberOfPlayers; i++)
        {
            m_Players[i].Score = 0;

            for (int j = 0; j < m_Players[i].RoundStats.Length; i++)
            {
                m_Players[i].RoundStats[j].Score = 0;
            }
        }
    }

    void AddScore(Notification message)
    {
        if (message.Data == null)
            return;

        if (m_CurrentPlayer > m_NumberOfPlayers)
        {
            Debug.LogError("Invalid current player: " + m_CurrentPlayer);
            return;
        }

        int score = 0;

        if (message.Data is int)
        {
            score = (int)message.Data;
        }

        CurrentPlayer.Score += score;
        CurrentPlayer.CurrentRoundStats.Score += score;
    }

	void StartNewGame()
	{
		for (int i = 0; i < m_NumberOfPlayers; i++)
		{
			m_Players[i].Initialize();
		}
	}

    void WinRound()
    {
        CurrentPlayer.CurrentRoundStats.WonRound = true;
    }
    
    void LoseRound()
    {
        CurrentPlayer.CurrentRoundStats.WonRound = false;
    }
}
