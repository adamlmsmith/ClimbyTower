using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dropper : MonoBehaviour
{
    public List<DroppedObject> m_DroppedObjects = new List<DroppedObject>();
    public Transform m_Hand;

    GameObject m_ObjectToDrop;

    [Header("Audio")]
    public AudioSource m_DropAudio;

    public void AddObject()
    {
        m_ObjectToDrop = GameObject.Instantiate(m_DroppedObjects[Random.Range (0, m_DroppedObjects.Count)].gameObject) as GameObject;
        m_ObjectToDrop.transform.SetParent(m_Hand);
        m_ObjectToDrop.transform.localPosition = Vector3.zero;
        m_ObjectToDrop.GetComponent<SpriteRenderer>().sortingLayerName = "Dropper";
    }

    public void DropObject()
    {
        if (m_ObjectToDrop == null)
        {
            m_ObjectToDrop = GameObject.Instantiate(m_DroppedObjects [Random.Range(0, m_DroppedObjects.Count)].gameObject) as GameObject;
        }

        if (m_ObjectToDrop != null)
        {
            Vector3 tempPosition = transform.position;
            tempPosition.z = 0.0f;
            m_ObjectToDrop.transform.position = tempPosition;

            // Set velocity
            m_ObjectToDrop.GetComponent<DroppedObject>().Velocity = new Vector3(0.0f, -2.0f, 0.0f);
            m_ObjectToDrop.GetComponent<DroppedObject>().Dropped = true;
            m_ObjectToDrop.GetComponent<DroppedObject>().Armed = true;

            m_ObjectToDrop.GetComponent<SpriteRenderer>().sortingLayerName = "Projectile";

            m_ObjectToDrop.transform.SetParent(GameVariables.instance.Board.transform);

            if(m_DropAudio)
            {
                m_DropAudio.pitch = Random.Range(0.75f, 1.3f);
                m_DropAudio.Play();
            }
        }

        m_ObjectToDrop = null;
    }

    public void SelfDestroy()
    {
        Destroy (gameObject);
    }
}
