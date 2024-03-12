using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class Board : MonoBehaviour
{
    [SerializeField]
    GameCamera m_GameCamera = null;

    BuildingManager m_BuildingManager = null;

    public Climber Climber { get; private set; }
    public GameCamera GameCamera
    {
        get { return m_GameCamera; }
        private set { m_GameCamera = value; }
    }

    void Awake()
    {
        GameVariables.instance.Board = this;

		NotificationCenter.DefaultCenter().AddObserver(gameObject, "StartRound");
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "WinRound");
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "LoseRound");

        m_BuildingManager = GetComponent<BuildingManager>();
    }

    void SpawnClimber()
    {
        Climber = GameObject.Instantiate(GameVariables.instance.ClimberManager.GetCurrentClimber());
        Climber.transform.SetParent(transform);
        Climber.ChangedPosition.AddListener(ClimberChangedPosition);
    }

    void StartRound()
	{
        SpawnClimber();
        m_BuildingManager.Initialize();
        SetClimberBoardPosition(GameVariables.instance.PlayerManager.CurrentPlayer.BoardPosition);
	}

    void WinRound()
	{
        GameVariables.instance.PlayerManager.CurrentPlayer.CurrentRoundStats.WonRound = true;
	}

    void LoseRound()
	{
        GameVariables.instance.PlayerManager.CurrentPlayer.CurrentRoundStats.WonRound = false;
	}

    public void SetClimberBoardPosition(Vector2 climberPos, bool animate=false)
    {
        Climber.transform.position = m_BuildingManager.GetNodePosition(climberPos) + new Vector3(0.0f, 0.0f, -1.0f);
        m_BuildingManager.QueueNewFloors(climberPos);
        GameVariables.instance.PlayerManager.CurrentPlayer.BoardPosition = climberPos;

        m_BuildingManager.ClimberPositionChanged();
        m_GameCamera.SetFloorCenter(m_BuildingManager.GetFloor ((int)GameVariables.instance.PlayerManager.CurrentPlayer.HighestBoardPosition.y).GetCenterPosition());
    }

    public void ClimberChangedPosition()
    {
        Vector2 climberBoardPosition = new Vector3(Mathf.Round (Climber.transform.position.x), Mathf.Round (Climber.transform.position.y));
        GameVariables.instance.PlayerManager.CurrentPlayer.BoardPosition = climberBoardPosition;

        m_BuildingManager.RemoveOldFloors(climberBoardPosition, 6); // TODO Magic number
        SetClimberBoardPosition(climberBoardPosition);
    }

}