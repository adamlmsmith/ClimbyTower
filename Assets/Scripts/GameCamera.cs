using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour
{
    Vector3 m_OffsetFromClimber = new Vector3(0.0f, 2.0f, 0.0f);
    Vector3 m_CameraCreepSpeed = new Vector3(0.0f, 0.2f, 0.0f);
    Vector3 m_FloorCenter;
    Vector3 m_TargetPosition;

    Vector3 m_Velocity = Vector3.zero;
    float m_SmoothTime = 0.5f;

    bool m_IsRunning = false;
    float m_OrthographicSize;

    void Awake()
    {
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "ValidInput");
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "LoseGame");

        m_OrthographicSize = GetComponent<Camera>().orthographicSize;
    }

    void Start()
    {
        m_TargetPosition = transform.position;
        m_IsRunning = false;
    }

    void Update()
    {
        Climber climber = GameVariables.instance.Board.Climber;

        if (climber == null)
            return;

        if(climber.transform.position.y > (transform.position.y - m_OffsetFromClimber.y))
        {
            m_TargetPosition = new Vector3(m_FloorCenter.x, climber.transform.position.y + m_OffsetFromClimber.y, transform.position.z);
        }
        else
        {
            if(m_IsRunning)
                m_TargetPosition += m_CameraCreepSpeed * Time.deltaTime;
        }

        transform.position = Vector3.SmoothDamp(transform.position, m_TargetPosition, ref m_Velocity, m_SmoothTime);

        if (Mathf.Approximately(transform.position.y, m_TargetPosition.y))
            transform.position = m_TargetPosition;

        if ((climber.transform.position.y - 0.9f) < (transform.position.y - m_OrthographicSize))
        {
            climber.TriggerDeath();
        }
    }

    public void SetFloorCenter(Vector3 floorCenter)
    {
        m_FloorCenter = floorCenter;
    }

    void ValidInput()
    {
        m_IsRunning = true;
    }

    void LoseGame()
    {
        m_IsRunning = false;
    }
}