using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    BuildingManager m_BuildingManager = null;

    bool m_IsRunning = false;

    List<GameObject> m_ActiveDroppers = new List<GameObject>();
    List<GameObject> m_ActiveShooters = new List<GameObject>();
    public Dropper m_DropperPrefab;
    public Biplane m_BiplanePrefab;
    public Shooter m_ShooterPrefab;

    float m_TimeUntilNextBiplane = 0.0f;

    float m_MinClosingWindowDelay = 0.15f;
    float m_MaxClosingWindowDelay = 1.0f;

    float m_MinNewDropperDelay = 0.15f;
    float m_MaxNewDropperDelay = 1.0f;

    float m_MinNewShooterDelay = 0.15f;
    float m_MaxNewShooterDelay = 1.0f;

    void Awake()
    {
        m_BuildingManager = GetComponent<BuildingManager>();
        m_BuildingManager.newFloorSpawned.AddListener(NewFloorSpawned);
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "ValidInput");

        m_IsRunning = false;

        UpdateClosingWindows();
        UpdateDroppers();
        UpdateShooters();
    }

    void Update()
    {
        if(m_IsRunning)
        {
            int highestFloorNumber = (int)GameVariables.instance.PlayerManager.CurrentPlayer.HighestBoardPosition.y;

            // BIPLANES ////////////////////////

            float currentBiplaneFrequency = GameVariables.instance.DifficultyList.BiplaneFrequency.Evaluate(highestFloorNumber);
            if(currentBiplaneFrequency > 0)
            {
                m_TimeUntilNextBiplane -= Time.deltaTime;

                if(m_TimeUntilNextBiplane > currentBiplaneFrequency)
                    m_TimeUntilNextBiplane = currentBiplaneFrequency;

                if(m_TimeUntilNextBiplane <= 0.0f)
                {
                    m_TimeUntilNextBiplane = currentBiplaneFrequency;
                    CreateBiplane();
                }
            }
        }

        if(Input.GetKeyDown (KeyCode.B))
        {
            CreateBiplane();
        }
    }

    void UpdateClosingWindows()
    {
        if (m_IsRunning)
        {
            int highestFloorNumber = (int)GameVariables.instance.PlayerManager.CurrentPlayer.HighestBoardPosition.y;
            
            if (m_BuildingManager.GetClosingWindowCount() < GameVariables.instance.DifficultyList.NumClosingWindows.Evaluate(highestFloorNumber))
            {
                CloseRandomWindow();
            }
        }

        Invoke("UpdateClosingWindows", Random.Range(m_MinClosingWindowDelay, m_MaxClosingWindowDelay));
    }

    void UpdateDroppers()
    {
        if (m_IsRunning)
        {
            int highestFloorNumber = (int)GameVariables.instance.PlayerManager.CurrentPlayer.HighestBoardPosition.y;

            for (int i = m_ActiveDroppers.Count - 1; i >= 0; i--)
            {
                if (m_ActiveDroppers [i] == null)
                    m_ActiveDroppers.RemoveAt(i);
            }
            
            if (m_ActiveDroppers.Count < GameVariables.instance.DifficultyList.NumDroppers.Evaluate(highestFloorNumber))
            {
                CreateDropper();
            }
        }

        Invoke("UpdateDroppers", Random.Range(m_MinNewDropperDelay, m_MaxNewDropperDelay));
    }

    void UpdateShooters()
    {
        if (m_IsRunning)
        {
            int highestFloorNumber = (int)GameVariables.instance.PlayerManager.CurrentPlayer.HighestBoardPosition.y;

            for (int i = m_ActiveShooters.Count - 1; i >= 0; i--)
            {
                if (m_ActiveShooters [i] == null)
                    m_ActiveShooters.RemoveAt(i);
            }
            
            if (m_ActiveShooters.Count < GameVariables.instance.DifficultyList.NumShooters.Evaluate(highestFloorNumber))
            {
                CreateShooter();
            }
        }

        Invoke("UpdateShooters", Random.Range(m_MinNewShooterDelay, m_MaxNewShooterDelay));
    }

    BuildingNode GetNodeForNewEnemy()
    {
        List<Floor> m_ValidFloors = m_BuildingManager.GetAwakeFloors();
        List<BuildingNode> m_ValidNodes = new List<BuildingNode>();
        
        for(int i = 0; i < m_ValidFloors.Count; i++)
        {
            // Only spawn any enemies above the player 
            if(m_ValidFloors[i].FloorNumber > GameVariables.instance.PlayerManager.CurrentPlayer.BoardPosition.y)
            {
                m_ValidNodes.AddRange(m_ValidFloors[i].Nodes.FindAll(x => x.GetWindowState() == Window.WindowStates.OPEN));
            }
        }
        
        BuildingNode randomNode = null;
        bool foundAcceptableNode = false;
        
        while(foundAcceptableNode == false && m_ValidNodes.Count > 0)
        {
            int randomIndex = Random.Range (0, m_ValidNodes.Count);
            
            if(m_ValidNodes[randomIndex].GetComponentInChildren<Dropper>() != null)
            {
                m_ValidNodes.RemoveAt(randomIndex);
            }
            else if(m_ValidNodes[randomIndex].GetComponentInChildren<Shooter>() != null)
            {
                m_ValidNodes.RemoveAt(randomIndex);
            }
            else
            {
                randomNode = m_ValidNodes[randomIndex];
                foundAcceptableNode = true;
            }
        }
        
        return randomNode;
    }

    void NewFloorSpawned(Floor floor)
    {
        float randomRange = Random.Range(0, 1.0f);
        if (randomRange < GameVariables.instance.DifficultyList.ChanceOfProximityMine.Evaluate(floor.FloorNumber))
        {
            CreatePowerLine(floor);
        }

        for(int i = 0; i < floor.GetNodeCount(); i++)
        {
            randomRange = Random.Range(0, 1.0f);
            if(randomRange < GameVariables.instance.DifficultyList.ChanceOfProximityMine.Evaluate(floor.FloorNumber))
            {
                BuildingNode node = floor.GetNode(i);

                if(node != null && node.IsClimbable)
                    CreateProximityMine(floor.GetNode(i));
            }
        }
    }
    
    void CloseRandomWindow()
    {
        List<Floor> m_ValidFloors = m_BuildingManager.GetAwakeFloors();
        List<BuildingNode> m_ValidNodes = new List<BuildingNode>();
        
        for(int i = 0; i < m_ValidFloors.Count; i++)
        {
            m_ValidNodes.AddRange(m_ValidFloors[i].Nodes.FindAll(x => x.GetWindowState() == Window.WindowStates.OPEN));
        }       
        
        BuildingNode randomNode = m_ValidNodes[Random.Range (0, m_ValidNodes.Count)];
        randomNode.CloseWindow();
    }

    void CreateDropper()
    {
        BuildingNode randomNode = null;
        randomNode = GetNodeForNewEnemy();

        if(randomNode != null)
        {
            GameObject newDropper = GameObject.Instantiate(m_DropperPrefab.gameObject);
            newDropper.transform.SetParent(randomNode.transform);
            newDropper.transform.localPosition = new Vector3(0.0f, -0.5f, 0.0f);

            m_ActiveDroppers.Add (newDropper);
        }
    }

    void CreateShooter()
    {
        BuildingNode randomNode = null;
        randomNode = GetNodeForNewEnemy();
        
        if(randomNode != null)
        {
            GameObject newShooter = GameObject.Instantiate(m_ShooterPrefab.gameObject);
            newShooter.transform.SetParent(randomNode.transform);
            newShooter.transform.localPosition = Vector3.zero;
            
            m_ActiveShooters.Add (newShooter);
        }
    }

    void CreateBiplane()
    {
        GameObject.Instantiate(m_BiplanePrefab.gameObject);
    }

    void CreatePowerLine(Floor floor)
    {
        PowerLine powerLine = ObjectPool.instance.GetObjectForType("PowerLine", false).GetComponent<PowerLine>();
        powerLine.transform.position = floor.GetCenterPosition();
        powerLine.transform.SetParent(floor.transform);
        powerLine.name = "PowerLine";

        floor.PowerLine = powerLine;
    }

    void CreateProximityMine(BuildingNode node)
    {
        ProximityMine proximityMine = ObjectPool.instance.GetObjectForType("ProximityMine", false).GetComponent<ProximityMine>();
        proximityMine.transform.SetParent(node.transform);
        proximityMine.transform.localPosition = Vector3.zero;
        proximityMine.name = "ProximityMine";

        node.ProximityMine = proximityMine;
    }

    void ValidInput()
    {
        m_IsRunning = true;
    }
}
