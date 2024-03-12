using UnityEngine;
using System.Collections;

public class SelfDestroy : MonoBehaviour
{
    public float m_Lifetime = 1.0f;

    void Start()
    {
        Invoke("DestroyGameObject", m_Lifetime);
    }

    void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
