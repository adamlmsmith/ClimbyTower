using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Floor : MonoBehaviour
{
    // TODO Should there be both the Tiles List and the Nodes list?
    public List<GameObject> m_Tiles = new List<GameObject>();
    List<BuildingNode> m_Nodes = new List<BuildingNode>();

    bool m_IsAwake = false;

    public List<BuildingNode> Nodes { get { return m_Nodes; } }
    public int FloorNumber { get; set; }

    public PowerLine PowerLine { get; set; }

    public bool IsAwake
    {
        get { return m_IsAwake; }
        set
        {
//            if(m_IsAwake != value)
//                AwakeStateChanged(value);
            m_IsAwake = value;

            if(PowerLine != null)
                PowerLine.IsAwake = m_IsAwake;
        }
    }

    public void Initialize()
    {
        for(int i = 0; i < m_Tiles.Count; i++)
        {
			BuildingNode newNode = ObjectPool.instance.GetObjectForType(m_Tiles[i].name, false).GetComponent<BuildingNode>();
            newNode.name = m_Tiles[i].name;
            newNode.transform.SetParent(transform);
            newNode.transform.localPosition = new Vector3(1.0f * i, 0.0f, 0.0f);
            m_Nodes.Add (newNode);
			newNode.Initialize();
        }
    }

    public void PoolFloor()
    {

        if (PowerLine != null)
        {
            PowerLine.transform.SetParent(null);
            PowerLine.gameObject.SetActive(false);
            ObjectPool.instance.PoolObject(PowerLine.gameObject);
            PowerLine = null;
        }

        for(int i = 0; i < m_Nodes.Count; i++)
        {
            m_Nodes[i].PrepareForPool();

            ObjectPool.instance.PoolObject(m_Nodes[i].gameObject);
        }
    }

    public int GetNodeCount()
    {
        return m_Nodes.Count;
    }

    public BuildingNode GetNode(int index)
    {
        if(index >= 0 && index < m_Nodes.Count)
        {
            return m_Nodes[index];
        }

        return null;
    }

    public void RefreshWindowSortingOrder(int highestFloorNumber)
    {
        for(int i = 0; i < m_Nodes.Count; i++)
        {
            m_Nodes[i].RefreshWindowSortingOrder(highestFloorNumber, FloorNumber);
        }
    }

//    void AwakeStateChanged(bool awake)
//    {
//        for(int i = 0; i < m_Nodes.Count; i++)
//        {
//            if(awake == false)
//            {
//                m_Nodes[i].OnSleep();
//            }
//            else
//            {
//                m_Nodes[i].OnWake();
//            }
//        }
//    }

    public Vector3 GetCenterPosition()
    {
        Vector3 centerPos = Vector3.zero;

        if(m_Nodes.Count == 0)
        {
            Debug.LogError("ERROR - Getting the center position of a floor with no nodes!");
            return centerPos;
        }

        centerPos.x = (m_Nodes[0].transform.position.x + m_Nodes[m_Nodes.Count - 1].transform.position.x) / 2;
        centerPos.y = (m_Nodes[0].transform.position.y + m_Nodes[m_Nodes.Count - 1].transform.position.y) / 2;
        centerPos.z = (m_Nodes[0].transform.position.z + m_Nodes[m_Nodes.Count - 1].transform.position.z) / 2;

        return centerPos;
    }

    public void SetTileColor(int color)
    {
        for(int i = 0; i < m_Nodes.Count; i++)
        {
            Debug.Assert(color >= 0 && color < m_Nodes[i].buildingTiles.Count);
            m_Nodes[i].GetComponent<SpriteRenderer>().sprite = m_Nodes[i].buildingTiles[color];
        }
    }
}
