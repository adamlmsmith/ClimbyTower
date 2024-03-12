using UnityEngine;
using System.Collections;

public class Biplane : MonoBehaviour
{
    float m_LastPositionX;
    Vector3 m_OriginalLocalPosition;
    float m_SinWaveTimer;
    float m_BounceSpeed = 3.0f;
    float m_BounceDepth = 0.2f;

    public AudioSource m_BiplaneIdleAudio;
    public AnimationCurve m_BiplaneIdleVolumeCurve;

    void Start()
    {
        Camera gameCamera = GameVariables.instance.Board.GameCamera.GetComponent<Camera>();
        transform.SetParent(gameCamera.transform);

        if(Random.Range (0, 2) == 0)
        {
            // Moving Left
            transform.localPosition = new Vector3(14.0f, gameCamera.orthographicSize * 0.7f, transform.localPosition.z);
            transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            m_LastPositionX = transform.position.x;
        }
        else
        {
            // Moving Right
            transform.localPosition = new Vector3(-14.0f, gameCamera.orthographicSize * 0.7f, transform.localPosition.z);
            transform.localEulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
            m_LastPositionX = transform.position.x;
        }

        m_OriginalLocalPosition = transform.localPosition;
        m_SinWaveTimer += Random.Range(0.0f, m_BounceSpeed);

        m_BiplaneIdleAudio.volume = 0.0f;
    }

    void Update()
    {  
        m_SinWaveTimer += Time.deltaTime * m_BounceSpeed;
        transform.localPosition = new Vector3(transform.localPosition.x, m_OriginalLocalPosition.y + (Mathf.Sin(m_SinWaveTimer) * m_BounceDepth), transform.localPosition.z);

        if (transform.position.x > 0)
        {
            // Only drop bombs over every other window, which windows depend on the direction the biplane is flying.
            if ((int)transform.position.x != m_LastPositionX)
            {
                m_LastPositionX = (int)transform.position.x;

                if ((m_LastPositionX % 2) == 0)
                {
                    // TODO improve this, get rid of magic numbers
                    if(m_LastPositionX >= 0 && m_LastPositionX <= 5)
                    {
                        DropBomb();
                        Invoke("DropBomb", 0.2f);
                    }
                }
            }
        }

        m_BiplaneIdleAudio.volume = m_BiplaneIdleVolumeCurve.Evaluate(transform.position.x);

    }

    void DropBomb()
    {
        GetComponent<Dropper>().DropObject();
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.name == "GameCamera")
        {
            Invoke("DestroySelf", 2.0f);
        }
    }

    void DestroySelf()
    {
        Destroy (gameObject);
    }
}
