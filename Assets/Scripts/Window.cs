using UnityEngine;
using System.Collections;

public class Window : MonoBehaviour
{
    public enum WindowStates { NONE, OPEN, OPENING, CLOSED, CLOSING };
    public WindowStates WindowState { get; set; }


    void Awake()
    {
        WindowState = WindowStates.OPEN;
    }

    void Update()
    {
        if(WindowState == WindowStates.CLOSING)
        {
            if(IsClosed ())
                WindowState = WindowStates.CLOSED;
        }
        else if(WindowState == WindowStates.CLOSED)
        {
            GetComponent<Animator>().SetBool ("Open", true);
            WindowState = WindowStates.OPENING;
        }
        else if(WindowState == WindowStates.OPENING)
        {
            if(IsOpen ())
                WindowState = WindowStates.OPEN;
        }
    }

    bool IsClosed()
    {
        if(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("WindowClosed"))
        {
            if(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                return true;
            }
        }
          
        return false;
    }
    
    bool IsOpen()
    {
        if(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("WindowOpen"))
        {
            if(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                return true;
            }
        }
        
        return false;
    }

    public void Close()
    {
        GetComponent<Animator>().SetBool ("Open", false);
        WindowState = WindowStates.CLOSING;
    }
}
