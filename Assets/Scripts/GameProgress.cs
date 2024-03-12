

using System;
using UnityEngine;

public class GameProgress
{
    private const string PlayerPrefsKey = "climbytower-game-progress";

    //public const int LevelCount = 12;
    //private PilotStats[] mPilotStats = new PilotStats[LevelCount];

    //private int mPilotExp = 0;
    // pilot experience points
    private CostumeProgress m_CostumeProgress;

    private TimeSpan m_PlayingTime;
    private DateTime m_LoadedTime;

    public string SelectedClimber { get; set; }
    public int Coins { get; set; }
    public int TopScore { get; set; }
    public bool ActiveQuest { get; set; }
    public int CurrentQuest { get; set; }
    public int CurrentQuestValue { get; set; }
    public DateTime DateTimeOfLastQuest { get; set; }
    public DateTime DateTimeOfNextQuest { get; set; }
    public DateTime DateTimeOfLastGift { get; set; }

    // do we have modifications to write to disk/cloud?
    private bool m_Dirty = false;

    static SafePlayerPrefs spp = new SafePlayerPrefs(";gk4n5_A-f&zDLS", "climbytower-game-progress");

    public GameProgress()
    {
        m_CostumeProgress = new CostumeProgress();
        m_LoadedTime = DateTime.Now;

//        mProgress = new LevelProgress[LevelCount];
//        int i;
//        for (i = 0; i < LevelCount; i++)
//        {
//            mProgress[i] = new LevelProgress();
//        }
//        for (i = 0; i < LevelCount; i++)
//        {
//            mPilotStats[i] = new PilotStats(i);
//        }
    }

//    public LevelProgress GetLevelProgress(int level)
//    {
//        return level >= 0 && level < LevelCount ? mProgress[level] : null;
//    }

//    public void SetLevelProgress(int level, int score, int stars)
//    {
//        if (level >= 0 && level < LevelCount)
//        {
//            if (mProgress[level].Score < score)
//            {
//                mProgress[level].Score = score;
//                mDirty = true;
//            }
//            if (mProgress[level].Stars < stars)
//            {
//                mProgress[level].Stars = stars;
//                mDirty = true;
//            }
//        }
//    }

    public static GameProgress LoadFromDisk()
    {
        string s = PlayerPrefs.GetString(PlayerPrefsKey, "");
        //Debug.Log("LoadFromDisk: " + s);
        if (s == null || s.Trim().Length == 0)
        {
            GameProgress gp = new GameProgress();
            gp.SaveToDisk();
            spp.Save();
            return gp;
        }

        if (!spp.HasNotBeenEdited())
        {
            Debug.LogError("HAS BEEN EDITED!!!!!!!!!!!!");
            Debug.Break();
            GameProgress gp = new GameProgress();
            gp.SaveToDisk();
            spp.Save();
            return gp;
        }

        return GameProgress.FromString(s);
    }

    public static GameProgress FromBytes(byte[] b)
    {
        return GameProgress.FromString(System.Text.ASCIIEncoding.Default.GetString(b));
    }

    public void SaveToDisk()
    {
        PlayerPrefs.SetString(PlayerPrefsKey, ToString());
        spp.Save();
        //Debug.Log("SaveToDisk: " + ToString());
        m_Dirty = false;
    }

    public void MergeWith(GameProgress other)
    {
        m_CostumeProgress.MergeWith(other.m_CostumeProgress);

//        int i;
//        for (i = 0; i < LevelCount; i++)
//        {
//            if (mProgress[i].MergeWith(other.mProgress[i]))
//            {
//                mDirty = true;
//            }
//        }
//        if (other.mPilotExp > mPilotExp)
//        {
//            mPilotExp = other.mPilotExp;
//            mDirty = true;
//        }

        Coins = Mathf.Max(Coins, other.Coins);
        TopScore = Mathf.Max(TopScore, other.TopScore);

        if (other.m_PlayingTime > m_PlayingTime)
        {
            SelectedClimber = other.SelectedClimber;
            ActiveQuest = other.ActiveQuest;
            CurrentQuest = other.CurrentQuest;
            CurrentQuestValue = other.CurrentQuestValue;
            DateTimeOfLastQuest = other.DateTimeOfLastQuest;
            DateTimeOfNextQuest = other.DateTimeOfNextQuest;
            DateTimeOfLastGift = other.DateTimeOfLastGift;
            m_PlayingTime = other.m_PlayingTime;
        }
    }

    public TimeSpan TotalPlayingTime
    {
        get
        {
            TimeSpan delta = DateTime.Now.Subtract(m_LoadedTime);
            return m_PlayingTime.Add(delta);
        }
    }

    public override string ToString()
    {
        string s = "GPv1:" + m_CostumeProgress.ToString();
//        string s = "GPv3:" + mPilotExp.ToString();
//        int i;
//        for (i = 0; i < LevelCount; i++)
//        {
//            s += ":" + mProgress[i].ToString();
//        }
        s += ":" + SelectedClimber;
        s += ":" + Coins;
        s += ":" + TopScore;
        s += ":" + (ActiveQuest ? 1 : 0);
        s += ":" + CurrentQuest;
        s += ":" + CurrentQuestValue;
        s += ":" + DateTimeOfLastQuest.ToBinary().ToString();
        s += ":" + DateTimeOfNextQuest.ToBinary().ToString();
        s += ":" + DateTimeOfLastGift.ToBinary().ToString();
        s += ":" + TotalPlayingTime.TotalMilliseconds;
        return s;
    }

    public byte[] ToBytes()
    {
        return System.Text.ASCIIEncoding.Default.GetBytes(ToString());
    }

    public static GameProgress FromString(string s)
    {
        GameProgress gp = new GameProgress();
        string[] p = s.Split(new char[] { ':' });
        if (!p[0].StartsWith("GPv"))
        {
            Debug.LogError("Failed to parse game progress from: " + s);
            return gp;
        }

        gp.m_CostumeProgress.SetFromString(p [1]);

//        gp.mPilotExp = System.Convert.ToInt32(p[1]);
//        int i;
//        for (i = 2; i < p.Length && i - 2 < LevelCount; i++)
//        {
//            gp.GetLevelProgress(i - 2).SetFromString(p[i]);
//        }

        gp.SelectedClimber = p[2];
        gp.Coins = System.Convert.ToInt32(p[3]);
        gp.TopScore = System.Convert.ToInt32(p[4]);
        gp.ActiveQuest = (System.Convert.ToInt32(p [5]) == 1);
        gp.CurrentQuest = System.Convert.ToInt32(p [6]);
        gp.CurrentQuestValue = System.Convert.ToInt32(p [7]);
        gp.DateTimeOfLastQuest = DateTime.FromBinary(Convert.ToInt64(p[8]));
        gp.DateTimeOfNextQuest = DateTime.FromBinary(Convert.ToInt64(p[9]));
        gp.DateTimeOfLastGift = DateTime.FromBinary(Convert.ToInt64(p[10]));
          

        if (p[0].Equals("GPv1"))
        {
            double val = Double.Parse(p[11]);
            gp.m_PlayingTime = TimeSpan.FromMilliseconds(val > 0f ? val : 0f);
        }
        else
        {
            gp.m_PlayingTime = new TimeSpan();
        }

        gp.m_LoadedTime = DateTime.Now;
        return gp;
    }

    public void UnlockCostume(string climberName)
    {
        m_CostumeProgress.UnlockCostume(climberName);
    }

    public bool IsCostumeUnlocked(string climberName)
    {
        return m_CostumeProgress.IsCostumeUnlocked(climberName);
    }

//    public bool IsLevelUnlocked(int level)
//    {
//        LevelProgress prev = GetLevelProgress(level - 1);
//        return level == 0 || (prev != null && prev.Cleared);
//    }
//
//    public bool AreAllLevelsCleared()
//    {
//        int i;
//        for (i = 0; i < LevelCount; i++)
//        {
//            if (!GetLevelProgress(i).Cleared)
//            {
//                return false;
//            }
//        }
//        return true;
//    }

//    public int PilotExperience
//    {
//        get
//        {
//            return mPilotExp;
//        }
//    }

    public bool Dirty
    {
        get
        {
            return m_Dirty;
        }
        set
        {
            m_Dirty = value;
        }
    }

//    public int PilotLevel
//    {
//        get
//        {
//            return GetPilotLevel(mPilotExp);
//        }
//    }

//    public bool AddPilotExperience(int points)
//    {
//        if (points > 0)
//        {
//            int levelBefore = PilotLevel;
//            mPilotExp += points;
//            mDirty = true;
//            return PilotLevel > levelBefore;
//        }
//        else
//        {
//            return false;
//        }
//    }

//    public static int GetPilotLevel(int expPoints)
//    {
//        int i;
//        for (i = GameConsts.Progression.ExpForLevel.Length - 1; i >= 0; --i)
//        {
//            if (GameConsts.Progression.ExpForLevel[i] <= expPoints)
//            {
//                break;
//            }
//        }
//        return Mathf.Clamp(i, 1, GameConsts.Progression.MaxLevel);
//    }
//
//    public bool IsMaxLevel()
//    {
//        return PilotLevel >= GameConsts.Progression.MaxLevel;
//    }
//
//    public int GetExpForNextLevel()
//    {
//        return IsMaxLevel() ? -1 : GameConsts.Progression.ExpForLevel[PilotLevel + 1];
//    }

//    public PilotStats CurPilotStats
//    {
//        get
//        {
//            return mPilotStats[PilotLevel];
//        }
//    }
//
//    public int TotalScore
//    {
//        get
//        {
//            int sum = 0;
//            foreach (LevelProgress lp in mProgress)
//            {
//                sum += lp.Score;
//            }
//            return sum;
//        }
//    }

//    public int TotalStars
//    {
//        get
//        {
//            int sum = 0;
//            foreach (LevelProgress lp in mProgress)
//            {
//                sum += lp.Stars;
//            }
//            return sum;
//        }
//    }
//
//    // Mostly for debug purposes
//    public void ForceLevelUp()
//    {
//        mPilotExp = GetExpForNextLevel();
//    }
//
//    // Mostly for debug purposes
//    public void ForceLevelDown()
//    {
//        int level = PilotLevel;
//        if (level > 1)
//        {
//            mPilotExp = GameConsts.Progression.ExpForLevel[level - 1];
//        }
//    }
}
