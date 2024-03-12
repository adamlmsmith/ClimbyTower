using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CostumeProgress
{
    //private int mScore, mStars;
    private HashSet<string> m_UnlockedCostumes = new HashSet<string>();


//    public int Score
//    {
//        get
//        {
//            return mScore;
//        }
//        set
//        {
//            mScore = value;
//        }
//    }
//    
//    public int Stars
//    {
//        get
//        {
//            return mStars;
//        }
//        set
//        {
//            mStars = value;
//        }
//    }
//        
//    public bool Cleared
//    {
//        get
//        {
//            return mScore > 0;
//        }
//    }
    
    public CostumeProgress()
    {
        m_UnlockedCostumes.Clear();
        //mScore = mStars = 0;
    }
    
//    public CostumeProgress(int score, int stars)
//    {
//        mScore = score;
//        mStars = stars;
//    }

    void PrintCostumeList()
    {
        string s = "";

        foreach (string costume in m_UnlockedCostumes)
        {
            s += costume + ", ";
        }

        Debug.Log(s);
    }

    public void UnlockCostume(string climberName)
    {
        m_UnlockedCostumes.Add(climberName);
        //PrintCostumeList();
    }

    public bool IsCostumeUnlocked(string climberName)
    {
        return m_UnlockedCostumes.Contains(climberName);
    }
    
    public override string ToString()
    {
        string returnString = "";

        foreach(string costume in m_UnlockedCostumes)
        {
            returnString += "|" + costume;
        }

        return string.Format("CP{0}", returnString);
    }
    
    public void SetFromString(string s)
    {
        string[] p = s.Split(new char[] { '|' });
        if (p.Length == 0 || !p[0].Equals("CP"))
        {
            Debug.LogError("Failed to parse costume progress from: " + s);
            //mStars = mScore = 0;
        }
        //mScore = Convert.ToInt32(p[1]);
        //mStars = Convert.ToInt32(p[2]);

        for(int i = 1; i < p.Length; i++)
        {
            m_UnlockedCostumes.Add(p[i]);
        }
    }
    
    public static CostumeProgress FromString(string s)
    {
        CostumeProgress cp = new CostumeProgress();
        cp.SetFromString(s);
        return cp;
    }
    
    public bool MergeWith(CostumeProgress other)
    {
        int previousCostumeCount = m_UnlockedCostumes.Count;

        foreach(string costume in other.m_UnlockedCostumes)
        {
            m_UnlockedCostumes.Add(costume);
        }

        if (previousCostumeCount != m_UnlockedCostumes.Count)
            return true;

        return false;
    }
}
