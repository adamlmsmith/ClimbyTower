using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingNode : MonoBehaviour
{
    public GameObject m_WindowPrefab = null;
    public GameObject m_CoinPrefab = null;

    public bool IsClimbable;
    public bool HasWindow;

    Window m_Window = null;

    public ProximityMine ProximityMine { get; set; }
    public Coin Coin { get; set; }
    public List<Sprite> buildingTiles = new List<Sprite>();
	

   	public void Initialize()
	{
		if (m_Window)
        {
            ObjectPool.instance.PoolObject(m_Window.gameObject);
            m_Window = null;
		}

		if(HasWindow)
		{	
			Window window = ObjectPool.instance.GetObjectForType("Window", false).GetComponent<Window>();
			window.name = m_WindowPrefab.name;
			window.transform.SetParent(transform);
            window.transform.localPosition = new Vector3(0.0f, 0.0f, 0.1f);

			m_Window = window;

            // This is a band-aid to get around a bug in Unity. Dynamic batching breaks if you alternate instantiating sprites, the fix is to slightly offset
            // the sprites in the z-axis, which makes Unity group those objects together. It won't work if they are all on the same plane.
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + 0.01f);
		}
	}

    public void RefreshWindowSortingOrder(int highestFloorNumber, int floorNumber)
    {
        if(m_Window)
        {
            m_Window.transform.Find("Pane").GetComponent<SpriteRenderer>().sortingOrder = (-4 * (highestFloorNumber - floorNumber + 1)) + 3;
            m_Window.transform.Find("Background").GetComponent<SpriteRenderer>().sortingOrder = (-4 * (highestFloorNumber - floorNumber + 1));
        }

        if(ProximityMine)
        {
            //ProximityMine.GetComponent<SpriteRenderer>().sortingOrder = (-3 * (highestFloorNumber - floorNumber + 1)) + 1;
            ProximityMine.SetSortingOrder((-4 * (highestFloorNumber - floorNumber + 1)) + 1);
        }
    }

    public Window.WindowStates GetWindowState()
    {
        if (m_Window)
            return m_Window.WindowState;
        else
            return Window.WindowStates.NONE;
    }

//    public void OnSleep()
//    {
//
//    }
//
//    public void OnWake()
//    {
//
//    }

    public void CloseWindow()
    {
        if (m_Window)
            m_Window.Close();
    }

    public bool SpawnCoin()
    {
        if(IsClimbable)
        {
            Coin newCoin = ObjectPool.instance.GetObjectForType("Coin", false).GetComponent<Coin>();
            newCoin.name = m_CoinPrefab.name;
            newCoin.transform.SetParent(transform);
            newCoin.transform.localPosition = new Vector3(0.0f, 0.0f, -0.1f);
            Coin = newCoin;
            return true;
        }

        return false;
    }

    public void PrepareForPool()
    {
        if (ProximityMine != null)
        {
            ProximityMine.transform.SetParent(null);
            ObjectPool.instance.PoolObject(ProximityMine.gameObject);
            ProximityMine = null;
        }

        if (Coin != null)
        {
            Coin.transform.SetParent(null);
            ObjectPool.instance.PoolObject(Coin.gameObject);
            Coin = null;
        }
    }
}
