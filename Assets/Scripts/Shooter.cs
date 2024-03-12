using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shooter : MonoBehaviour
{
    public List<DroppedObject> m_DroppedObjects = new List<DroppedObject>();
    public Transform m_GunBarrel;

    int shootDirection = 0;
    int numObjectsDropped = 0;

    public AudioSource m_ShootGunAudio;

    void Awake()
    {
        shootDirection = Random.Range (0, 2);

        if (shootDirection == 0)
        {
            transform.localEulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
        }

        m_ShootGunAudio.pitch = Random.Range(0.6f, 1.3f);
    }

    public void DropObject()
    {
        GameObject newDroppedObject = GameObject.Instantiate(m_DroppedObjects[Random.Range (0, m_DroppedObjects.Count)].gameObject) as GameObject;

        Vector3 tempPosition = m_GunBarrel.position;
        tempPosition.z = numObjectsDropped;
        newDroppedObject.transform.position = tempPosition;
        newDroppedObject.GetComponent<DroppedObject>().Dropped = true;
        newDroppedObject.GetComponent<DroppedObject>().Armed = true;

        newDroppedObject.GetComponent<DroppedObject>().Velocity = new Vector3(0.5f - shootDirection, -0.5f, 0.0f) * 2.0f;

        if (shootDirection == 0)
        {
            newDroppedObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, -135.0f);
        }
        else if (shootDirection == 1)
        {
            newDroppedObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 135.0f);
        }

        newDroppedObject.transform.SetParent(GameVariables.instance.Board.transform);

        m_ShootGunAudio.Play();
        numObjectsDropped++;
    }

    public void SelfDestroy()
    {
        Destroy (gameObject);
    }
}
