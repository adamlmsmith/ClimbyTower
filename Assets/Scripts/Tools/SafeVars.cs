using UnityEngine;
using System.Collections;


public struct SafeFloat 
{
    private float offset;
    private float value;
    
    public SafeFloat (float value = 0) {
        offset = Random.Range(-1000.0f, 1000.0f);
        this.value = value + offset;
    }
    
    public float GetValue ()
    {
        return value - offset;
    }
    
    public void Dispose ()
    {
        offset = 0.0f;
        value = 0.0f;
    }
    
    public override string ToString()
    {
        return GetValue().ToString();
    }
    
    public static SafeFloat operator +(SafeFloat f1, SafeFloat f2) {
        return new SafeFloat(f1.GetValue() + f2.GetValue());
    }
}

public struct SafeInt 
{
    private int offset;
    private int value;
    
    public SafeInt (int value = 0) {
        offset = Random.Range(-10000, 10000);
        this.value = value + offset;
    }
    
    public int GetValue ()
    {
        return value - offset;
    }
    
    public void Dispose ()
    {
        offset = 0;
        value = 0;
    }
    
    public override string ToString()
    {
        return GetValue().ToString();
    }
    
    public static SafeInt operator +(SafeInt f1, SafeInt f2) {
        return new SafeInt(f1.GetValue() + f2.GetValue());
    }
}