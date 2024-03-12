using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class PlayerInputArea : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool m_Touched;
    private int m_PointerID;
    Vector2 touchStartPosition;

    bool m_bInputEnabled;
    
    void Awake ()
    {
        m_Touched = false;
        m_bInputEnabled = false;

        NotificationCenter.DefaultCenter().AddObserver(gameObject, "EnablePlayerInput");
    }
    
    public void OnPointerDown(PointerEventData data)
    {
        if (!m_Touched && m_bInputEnabled)
        {
            m_Touched = true;
            m_PointerID = data.pointerId;

            //////////
            /// 
            touchStartPosition = (data.position / Mathf.Min(Screen.width, Screen.height));
        }
    }
    
    public void OnPointerUp(PointerEventData data)
    {
        if (data.pointerId == m_PointerID && m_Touched)
        {
            m_Touched = false;

            Vector2 touchVector = (data.position / Mathf.Min(Screen.width, Screen.height)) - touchStartPosition;
            
            if(touchVector.sqrMagnitude > 0.001f)
            {
                if(Mathf.Abs(touchVector.x) > Mathf.Abs(touchVector.y))
                {
                    if(touchVector.x > 0)
                    {
                        NotificationCenter.DefaultCenter().PostNotification(gameObject, "PlayerInputSwipeRight");
                    }
                    else
                    {
                        NotificationCenter.DefaultCenter().PostNotification(gameObject, "PlayerInputSwipeLeft");
                    }
                }
                else
                {
                    if(touchVector.y > 0)
                    {
                        NotificationCenter.DefaultCenter().PostNotification(gameObject, "PlayerInputSwipeUp");
                    }
                    else
                    {
                        NotificationCenter.DefaultCenter().PostNotification(gameObject, "PlayerInputSwipeDown");
                    }
                }
            }
            else
            {
                NotificationCenter.DefaultCenter().PostNotification(gameObject, "PlayerInputTap");
            }
        }
    }
    
    public bool IsTouched()
    {
        return m_Touched;
    }

    public void EnablePlayerInput(Notification message)
    {
        if (message.Data != null)
        { 
            m_bInputEnabled = (bool)message.Data;
        }
    }

#if UNITY_EDITOR
    void Update()
    {
        if(m_bInputEnabled)
        {
            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                NotificationCenter.DefaultCenter().PostNotification(gameObject, "PlayerInputSwipeUp");
            }
            else if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                NotificationCenter.DefaultCenter().PostNotification(gameObject, "PlayerInputSwipeDown");
            }
            else if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                NotificationCenter.DefaultCenter().PostNotification(gameObject, "PlayerInputSwipeLeft");
            }
            else if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                NotificationCenter.DefaultCenter().PostNotification(gameObject, "PlayerInputSwipeRight");
            }
        }
    }
#endif
}