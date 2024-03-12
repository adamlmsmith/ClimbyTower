using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BuildingManager : MonoBehaviour
{
    public List<GameObject> FloorGroupPrefabs = new List<GameObject>();
    public GameObject m_NodeHolder = null;

    public BuildingNode m_BuildingTileWindow;
    public BuildingNode m_BuildingTileNoWindow;

    Vector3 m_NodeSpacing = new Vector3(1.0f, 1.0f, 0.0f);

    List<Floor> m_Floors = new List<Floor>();

    List<Floor> m_PreviousAwakeFloors = new List<Floor>();
    List<Floor> m_AwakeFloors = new List<Floor>();

    const int StartingNumberOfFloorsToSpawn = 15;
    int NumFloorsSpawned = 0;
    int FloorsUntilNextCoin = 0;

    int tileColor;

    public UnityEventFloor newFloorSpawned = new UnityEventFloor();

    /////------------------

    void Awake()
    {
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "ProximityMineExploded");
    }

    public void Initialize()
    {
        FloorsUntilNextCoin = Random.Range (10, 20);
        NumFloorsSpawned = 0;

        tileColor = Random.Range (0, 2);

        for(int i = 0; i < StartingNumberOfFloorsToSpawn; i++)
        {
            AddFloor();
        }
    }

    bool VerifyNode(int index, ref Floor lowerFloor, ref Floor upperFloor, int direction)
    {
        if (index < 0 || index >= lowerFloor.m_Tiles.Count)
        {
            return false;
        }

        if (lowerFloor.m_Tiles [index].GetComponent<BuildingNode>().IsClimbable)
        {
            if (upperFloor.m_Tiles [index].GetComponent<BuildingNode>().IsClimbable)
            {
                return true;
            }
            else if (direction == 0)
                return(VerifyNode(index - 1, ref lowerFloor, ref upperFloor, -1) || VerifyNode(index + 1, ref lowerFloor, ref upperFloor, 1));
            else if (direction == -1)
                return VerifyNode(index - 1, ref lowerFloor, ref upperFloor, -1);
            else if (direction == 1)
                return VerifyNode(index + 1, ref lowerFloor, ref upperFloor, 1);
        } 
        else
        {
            if(direction == 0)
            {
                return true;
            }
        }

        return false;
    }

    bool VerifyPath(Floor lowerFloor, Floor upperFloor)
    {
        if (lowerFloor.m_Tiles.Count == 0)
            Debug.LogWarning("Trying to verify path before the tiles have been assigned to a floor");

        for (int i = 0; i < lowerFloor.m_Tiles.Count; i++)
        {
            if (VerifyNode(i, ref lowerFloor, ref upperFloor, 0) == false)
            {
                return false;
            }
        }

        return true;
    }

    void InsertFloorWithPath(Floor lowerFloor, Floor upperFloor)
    {
        GameObject newFloorObject = new GameObject();
        Floor newFloor = newFloorObject.AddComponent<Floor>();
        newFloor.name = "InsertedFloor" + NumFloorsSpawned;

        int leftMostWindowIndex = 0;
        int rightMostWindowIndex = lowerFloor.GetNodeCount() - 1;

        for(int i = 0; i < lowerFloor.GetNodeCount(); i++)
        {
            if(lowerFloor.m_Tiles[i].GetComponent<BuildingNode>().IsClimbable ||
               upperFloor.m_Tiles[i].GetComponent<BuildingNode>().IsClimbable)
            {
                leftMostWindowIndex = i;
                break;
            }
        }

        for (int i = lowerFloor.GetNodeCount() - 1; i >= 0; i--)
        {
            if(lowerFloor.m_Tiles[i].GetComponent<BuildingNode>().IsClimbable ||
               upperFloor.m_Tiles[i].GetComponent<BuildingNode>().IsClimbable)
            {
                rightMostWindowIndex = i;
                break;
            }
        }

        for (int i = 0; i < lowerFloor.GetNodeCount(); i++)
        {
            if(i >= leftMostWindowIndex && i <= rightMostWindowIndex)
            {
                newFloor.m_Tiles.Add(m_BuildingTileWindow.gameObject);
            }
            else
            {
                newFloor.m_Tiles.Add(m_BuildingTileNoWindow.gameObject);
            }
        }

        SpawnFloor(newFloor);
    }

    void SpawnFloor(Floor floor)
    {
        floor.transform.SetParent(m_NodeHolder.transform);
        floor.FloorNumber = NumFloorsSpawned;

        floor.transform.localPosition = new Vector3(0.0f, NumFloorsSpawned * m_NodeSpacing.y, 0.0f);
        m_Floors.Add(floor);

        floor.Initialize();
        floor.SetTileColor(tileColor);
        newFloorSpawned.Invoke(floor);
        
        NumFloorsSpawned++;

        if (FloorsUntilNextCoin == 0)
        {
            List<BuildingNode> climbableNodes =  floor.Nodes.FindAll(x => x.IsClimbable == true);
            Debug.Assert(climbableNodes.Count > 0);
            climbableNodes [Random.Range(0, climbableNodes.Count)].SpawnCoin();
            FloorsUntilNextCoin = Random.Range(10, 20);
        }
        else
        {
            FloorsUntilNextCoin--;
        }
    }

    void AddFloor()
    {
        int difficulty = GameVariables.instance.DifficultyList.GetFloorDifficulty(NumFloorsSpawned);

        FloorGroup floorGroupPrefab = GameVariables.instance.DifficultyList.GetNextFloorGroup(difficulty);


        int numRepeats = Random.Range(floorGroupPrefab.MinRepeat, floorGroupPrefab.MaxRepeat + 1);

        for (int i = 0; i < numRepeats; i++)
        {
            FloorGroup floorGroup = GameObject.Instantiate(floorGroupPrefab).GetComponent<FloorGroup>();

            Floor[] floors = floorGroup.GetComponentsInChildren<Floor>();

            for (int j = 0; j < floors.Length; j++)
            {
                if(m_Floors.Count > 0)
                {
                   if(VerifyPath(m_Floors[m_Floors.Count - 1], floors[j]) == false)
                        InsertFloorWithPath(m_Floors[m_Floors.Count - 1], floors[j]);
                    else if(VerifyPath(floors[j], m_Floors[m_Floors.Count - 1]) == false)
                        InsertFloorWithPath(m_Floors[m_Floors.Count - 1], floors[j]);
                }

                SpawnFloor(floors[j]);
            }

            Destroy (floorGroup.gameObject);
        }

        RefreshWindowPositions();
    }

    public BuildingNode GetNode(Vector2 boardPosition)
    {
        for(int i = 0; i < m_Floors.Count; i++)
        {
            if(m_Floors[i].FloorNumber == boardPosition.y)
            {
                if(boardPosition.x >= 0 && boardPosition.x < m_Floors[i].GetNodeCount ())
                {
                    return m_Floors[i].GetNode((int)boardPosition.x);
                }
                else
                {
                    Debug.LogError("Invalid Node requested from GetNode(" + boardPosition.x + ", " + boardPosition.y );
                }
            }
        }

        return null;
    }

    public Vector3 GetNodePosition(Vector2 boardPosition)
    {
        return GetNode (boardPosition).transform.position;
    }

    public void QueueNewFloors(Vector2 boardPosition)
    {
        if(boardPosition.y >= GetHighestFloorNumber() - 10)
        {
            for(int i = 0; i < 1; i++)
            {
                AddFloor();
            }
        }
    }

    public void RemoveOldFloors(Vector2 boardPosition, int padding)
    {
        for(int i = m_Floors.Count - 1; i >= 0; i--)
        {
            if(m_Floors[i].FloorNumber < boardPosition.y - padding)
            {
                m_Floors[i].PoolFloor();
                DestroyFloor(i);
            }
        }
    }

    void DestroyFloor(int index)
    {
        Destroy (m_Floors[index].gameObject);
        m_Floors.RemoveAt(index);
    }

    public Floor GetFloor(int floorNumber)
    {
        for(int i = 0; i < m_Floors.Count; i++)
        {
            if(m_Floors[i].FloorNumber  == floorNumber)
            {
                return m_Floors[i];
            }
        }

        return null;
    }

    int GetHighestFloorNumber()
    {
        int highestFloorNumber = 0;

        for(int i = 0; i < m_Floors.Count; i++)
        {
            if(m_Floors[i].FloorNumber > highestFloorNumber)
            {
                highestFloorNumber = m_Floors[i].FloorNumber;
            }
        }

        return highestFloorNumber;
    }

    void RefreshWindowPositions()
    {
        int highestFloorNumber = GetHighestFloorNumber();

        for(int i = 0; i < m_Floors.Count; i++)
        {
            m_Floors[i].RefreshWindowSortingOrder(highestFloorNumber);
        }
    }

    public bool IsWindowClosed(Vector2 boardPosition)
    {
        Floor floor = GetFloor((int)boardPosition.y);

        if(floor != null)
        {
            return (floor.GetNode((int)boardPosition.x).GetWindowState() == Window.WindowStates.CLOSED);
        }

        return false;
    }

    public List<Floor> GetFloors(int minFloorNumber, int maxFloorNumber)
    {
        List<Floor> floorsToReturn = new List<Floor>();

        for(int i = 0; i < m_Floors.Count; i++)
        {
            if(m_Floors[i].FloorNumber >= minFloorNumber && m_Floors[i].FloorNumber <= maxFloorNumber)
            {
                floorsToReturn.Add (m_Floors[i]);
            }
        }

        return floorsToReturn;
    }

    bool IsFloorAwake(int floorNumber)
    {
        for(int i = 0; i < m_Floors.Count; i++)
        {
            if(m_Floors[i].FloorNumber == floorNumber)
            {
                if(m_Floors[i].IsAwake)
                    return true;
            }
        }

        return false;
    }

    public List<Floor> GetAwakeFloors()
    {
        return m_AwakeFloors;
    }

    public bool IsValidMove(Vector2 boardPosition)
    {
        if(boardPosition.x < 0 || boardPosition.x >= GetFloor ((int)boardPosition.y).GetNodeCount())
        {
            return false;
        }

        if(IsFloorAwake ((int)boardPosition.y) == false)
        {
            return false;
        }

        if(GetNode (boardPosition).IsClimbable == false)
        {
            return false;
        }
           
        return true;
    }
    
    public void ClimberPositionChanged()
    {
        Vector2 highestClimberPos = GameVariables.instance.PlayerManager.CurrentPlayer.HighestBoardPosition;
          
        m_PreviousAwakeFloors = m_AwakeFloors;
        m_AwakeFloors = GetFloors((int)highestClimberPos.y - 4, (int)highestClimberPos.y + 8);  // TODO Magic numbers

        for(int i = 0; i < m_PreviousAwakeFloors.Count; i++)
        {
            if(m_AwakeFloors.Contains(m_PreviousAwakeFloors[i]) == false)
            {
                m_PreviousAwakeFloors[i].IsAwake = false;
            }
        }

        for(int i = 0; i < m_AwakeFloors.Count; i++)
        {
            if(m_PreviousAwakeFloors.Contains (m_AwakeFloors[i]) == false)
            {
                m_AwakeFloors[i].IsAwake = true;
            }
        }

        // Check for Proximity Mines
        BuildingNode climberNode = GetNode(GameVariables.instance.PlayerManager.CurrentPlayer.BoardPosition);

        ProximityMine proximityMine = climberNode.GetComponentInChildren<ProximityMine>();

        if (proximityMine != null)
        {
            proximityMine.TriggerExplosion();
        }

        // Check one node below the player too
        BuildingNode climberNodeBelow = GetNode(GameVariables.instance.PlayerManager.CurrentPlayer.BoardPosition + new Vector2(0.0f, -1.0f));

        if (climberNodeBelow != null)
        {
            ProximityMine proximityMineBelow = climberNodeBelow.GetComponentInChildren<ProximityMine>();
            
            if (proximityMineBelow != null)
            {
                proximityMineBelow.TriggerExplosion();
            }
        }

        // Check for Powerline
        Floor floor = GetFloor((int)GameVariables.instance.PlayerManager.CurrentPlayer.BoardPosition.y);

        if (floor.PowerLine != null)
        {
            DualPistolasAchievementManager.GetInstance().IncrementProgress(DualPistolasAchievementManager.DualPistolasAchievementId.ELECTRICIAN, 1);
            NotificationCenter.DefaultCenter().PostNotification(gameObject, "PlayerClimbedUnderPowerline");
        }
    }

    public int GetClosingWindowCount()
    {
        int numClosingWindows = 0;

        for(int i = 0; i < m_Floors.Count; i++)
        {
            for(int j = 0; j < m_Floors[i].GetNodeCount(); j++)
            {
                if(m_Floors[i].GetNode (j).GetWindowState() == Window.WindowStates.CLOSING)
                    numClosingWindows++;
            }
        }

        return numClosingWindows;
    }

    void ProximityMineExploded(Notification message)
    {
        GameObject proximityMine = (GameObject)message.Sender;

        Vector2 DistanceFromClimber = (Vector2)proximityMine.transform.position - GameVariables.instance.PlayerManager.CurrentPlayer.BoardPosition;

        if (Mathf.Abs(DistanceFromClimber.x) <= 1.0f &&
            Mathf.Abs(DistanceFromClimber.y) <= 1.0f)
        {
            GameVariables.instance.Board.Climber.TriggerDeath();
        }
    }
}