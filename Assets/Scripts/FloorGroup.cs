using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FloorGroup : MonoBehaviour
{
    public List<Floor> m_Floors = new List<Floor>();
    [SerializeField]
    int m_MinRepeat = 1;
    [SerializeField]
    int m_MaxRepeat = 1;

    public int MinRepeat { get { return m_MinRepeat; } set { m_MinRepeat = value; } }
    public int MaxRepeat { get { return m_MaxRepeat; } set { m_MaxRepeat = value; } }

    void Awake()
    {
        for(int i = 0; i < m_Floors.Count; i++)
        {
            Floor newFloor = GameObject.Instantiate(m_Floors[i]) as Floor;
            newFloor.name = m_Floors[i].name;
            newFloor.transform.SetParent(transform);
            newFloor.transform.localPosition = new Vector3(0.0f, 1.0f * i, 0.0f);
        }
    }
}
