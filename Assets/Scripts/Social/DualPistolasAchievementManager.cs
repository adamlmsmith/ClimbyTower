using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;

using System.Collections.Generic;
using System.Collections;
using System.Linq;

#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
using GooglePlayGames;
#endif

public class DualPistolasAchievementManager
{
    public enum DualPistolasAchievementId
    {
        HUNDO_CLUB,
        IM_FLOORED,
        ITS_RAINING,
        ITS_POURING,
        GAMER,
        VIDIOT,
        ELECTRICIAN,
        GOOD_SCORE,
        GREAT_SCORE,
        AWESOME_SCORE,
        WARDROBE_BEGINNER,
        WARDROBE_ADVANCED,
        COIN_COLLECTOR,
        COIN_EXPLOSION,
        RE_GIFTER,
        TEST_INCREMENT,
        TEST_INSTANT
    }

	private static DualPistolasAchievementManager m_Instance = null;
	private Dictionary<DualPistolasAchievementId, DualPistolasAchievement> AchievementIds = new Dictionary<DualPistolasAchievementId, DualPistolasAchievement>();
	private IAchievement[] m_Achievements = new IAchievement[0];

    // achievement increments we are accumulating locally, waiting to send to the games API
    private Dictionary<DualPistolasAchievementId,int> m_PendingIncrements = new Dictionary<DualPistolasAchievementId, int>();

    // list of achievements we know we have unlocked (to avoid making repeated calls to the API)
    private Dictionary<string,bool> m_UnlockedAchievements = new Dictionary<string, bool>();
	
	public static DualPistolasAchievementManager GetInstance()
	{
		if (null == m_Instance)
		{
			m_Instance = new DualPistolasAchievementManager();
		}
		
		return m_Instance;
	}

	public void Init()
	{
		LoadAchievements();
		InitLocalAchievements();
	}

	void LoadAchievements()
	{
		Social.LoadAchievements (achievements => {
			if (achievements.Length > 0)
				m_Achievements = achievements;
               
		});
	}

	void InitLocalAchievements()
	{
		#if UNITY_ANDROID
		AchievementIds = new Dictionary<DualPistolasAchievementId, DualPistolasAchievement>()
		{
            {DualPistolasAchievementId.HUNDO_CLUB, DualPistolasAchievement.Create(100, GPGSIds.achievement_hundo_club)},
            {DualPistolasAchievementId.IM_FLOORED, DualPistolasAchievement.Create(10000, GPGSIds.achievement_im_floored)},
            {DualPistolasAchievementId.ITS_RAINING, DualPistolasAchievement.Create(50, GPGSIds.achievement_its_raining)},
            {DualPistolasAchievementId.ITS_POURING, DualPistolasAchievement.Create(1000, GPGSIds.achievement_its_pouring)},
            {DualPistolasAchievementId.GAMER, DualPistolasAchievement.Create(10, GPGSIds.achievement_gamer)},
            {DualPistolasAchievementId.VIDIOT, DualPistolasAchievement.Create(1000, GPGSIds.achievement_vidiot)},
            {DualPistolasAchievementId.ELECTRICIAN, DualPistolasAchievement.Create(1000, GPGSIds.achievement_electrician)},
            {DualPistolasAchievementId.GOOD_SCORE, DualPistolasAchievement.Create(1, GPGSIds.achievement_good_score)},
            {DualPistolasAchievementId.GREAT_SCORE, DualPistolasAchievement.Create(1, GPGSIds.achievement_great_score)},
            {DualPistolasAchievementId.AWESOME_SCORE, DualPistolasAchievement.Create(1, GPGSIds.achievement_awesome_score)},
            {DualPistolasAchievementId.WARDROBE_BEGINNER, DualPistolasAchievement.Create(10, GPGSIds.achievement_wardrobe_beginner)},
            {DualPistolasAchievementId.WARDROBE_ADVANCED, DualPistolasAchievement.Create(20, GPGSIds.achievement_wardrobe_advanced)},
            {DualPistolasAchievementId.COIN_COLLECTOR, DualPistolasAchievement.Create(100, GPGSIds.achievement_coin_collector)},
            {DualPistolasAchievementId.COIN_EXPLOSION, DualPistolasAchievement.Create(1, GPGSIds.achievement_coin_explosion)},
            {DualPistolasAchievementId.RE_GIFTER, DualPistolasAchievement.Create(50, GPGSIds.achievement_regifter)},
            {DualPistolasAchievementId.TEST_INCREMENT, DualPistolasAchievement.Create(3, GPGSIds.achievement_test_increment)},
            {DualPistolasAchievementId.TEST_INSTANT, DualPistolasAchievement.Create(1, GPGSIds.achievement_test_instant)}
			};
		#elif UNITY_IOS
        AchievementIds = new Dictionary<DualPistolasAchievementId, DualPistolasAchievement>()
		{
            {DualPistolasAchievementId.HUNDO_CLUB, DualPistolasAchievement.Create(100, "HUNDO_CLUB")},
            {DualPistolasAchievementId.IM_FLOORED, DualPistolasAchievement.Create(10000, "IM_FLOORED")},
            {DualPistolasAchievementId.ITS_RAINING, DualPistolasAchievement.Create(50, "ITS_RAINING")},
            {DualPistolasAchievementId.ITS_POURING, DualPistolasAchievement.Create(1000, "ITS_POURING")},
            {DualPistolasAchievementId.GAMER, DualPistolasAchievement.Create(10, "GAMER")},
            {DualPistolasAchievementId.VIDIOT, DualPistolasAchievement.Create(1000, "VIDIOT")},
            {DualPistolasAchievementId.ELECTRICIAN, DualPistolasAchievement.Create(1000, "ELECTRICIAN")},
            {DualPistolasAchievementId.GOOD_SCORE, DualPistolasAchievement.Create(1, "GOOD_SCORE")},
            {DualPistolasAchievementId.GREAT_SCORE, DualPistolasAchievement.Create(1, "GREAT_SCORE")},
            {DualPistolasAchievementId.AWESOME_SCORE, DualPistolasAchievement.Create(1, "AWESOME_SCORE")},
            {DualPistolasAchievementId.WARDROBE_BEGINNER, DualPistolasAchievement.Create(10, "WARDROBE_BEGINNER")},
            {DualPistolasAchievementId.WARDROBE_ADVANCED, DualPistolasAchievement.Create(20, "WARDROBE_ADVANCED")},
            {DualPistolasAchievementId.COIN_COLLECTOR, DualPistolasAchievement.Create(100, "COIN_COLLECTOR")},
            {DualPistolasAchievementId.COIN_EXPLOSION, DualPistolasAchievement.Create(1, "COIN_EXPLOSION")},
            {DualPistolasAchievementId.RE_GIFTER, DualPistolasAchievement.Create(50, "RE_GIFTER")}
        };
        #else
        AchievementIds = new Dictionary<DualPistolasAchievementId, DualPistolasAchievement>();
		Debug.LogError("This platform is not supported for achievements");
		#endif
	}

    public void IncrementProgress(DualPistolasAchievementId achievementKey, int incrementValue = 1)
    {
        if (m_PendingIncrements.ContainsKey(achievementKey))
        {
            incrementValue += m_PendingIncrements[achievementKey];
        }

        m_PendingIncrements[achievementKey] = incrementValue;
        //Debug.Log("IncrementProgress() - " + achievementKey.ToString() + " incrementValue = " + incrementValue);
    }

    public void FlushProgress()
    {
        //Debug.Log("FlushProgress()");
        if (GameManager.instance.Authenticated)
        {
            foreach (DualPistolasAchievementId id in m_PendingIncrements.Keys)
            {
                SubmitProgress(id, m_PendingIncrements[id]);
            }

            m_PendingIncrements.Clear();
        }
    }

    public void SubmitProgress(DualPistolasAchievementId achievementKey, int incrementValue = 1)
	{
        //Debug.Log("SubmitProgress for " + achievementKey.ToString() + " incrementValue = " + incrementValue);
		DualPistolasAchievement dualPistolasAchievement = null;
		bool containsAchievement = false;

		if (AchievementIds.TryGetValue(achievementKey, out dualPistolasAchievement))
		{
			double incrementPercent = ((double)incrementValue / dualPistolasAchievement.NumSteps) * 100; 

			foreach (IAchievement achievement in m_Achievements)
			{
				if (achievement.id == dualPistolasAchievement.AchievementId)
				{
					incrementPercent += achievement.percentCompleted;
					achievement.percentCompleted = incrementPercent;
					containsAchievement = true;
					break;
				}
			}

			if (!containsAchievement)
				AddAchievementToCache(dualPistolasAchievement, incrementPercent);

			SubmitAchievementProgress(dualPistolasAchievement, incrementPercent, incrementValue);
		}
		else
		{
			Debug.LogError("There was no string found for achievement key " + achievementKey.ToString());
		}
	}

	/// <summary>
	/// Clears the user's sandbox achievem
	/// </summary>
	public void DebugClearAchievements()
	{
#if UNITY_IOS
		GameCenterPlatform.ResetAllAchievements ((bool success) => {

            Debug.Log ( success ? "ResetAllAchievements - Success!" : "ResetAllAchievements - Failure!");
            if(success)
            {
                for (int i = 0; i < m_Achievements.Length; i++)
                {
                    m_Achievements[i].percentCompleted = 0;
                }

                m_Achievements = new IAchievement[0];
                AchievementIds.Clear();
                Init();
                m_PendingIncrements.Clear();
            }
	    });
#elif UNITY_ANDROID

#endif

        m_UnlockedAchievements.Clear();
	}

	void SubmitAchievementProgress(DualPistolasAchievement dualPistolasAchievement, double incrementPercent, int incrementValue)
	{
        //Debug.Log("-----> ReportingProgress for : " + dualPistolasAchievement.AchievementId.ToString() + " increment Percent : " + incrementPercent);
		#if UNITY_IPHONE

 
        GKAchievementReporter.ReportAchievement(dualPistolasAchievement.AchievementId, (float)incrementPercent, true);

		#elif UNITY_ANDROID
		//If your achievement is incremental, the Play Games implementation of Social.ReportProgress 
		//will try to behave as closely as possible to the expected behavior according to Unity's social API, 
		//but may not be exact. Instead, use the PlayGamesPlatform.IncrementAchievement method, 
		//which is a Play Games extension. - https://github.com/playgameservices/play-games-plugin-for-unity
		if (dualPistolasAchievement.NumSteps > 1)
        {
//            Debug.Log(".......................................................Increment " + 
//                      AchievementIds.FirstOrDefault(x => x.Value == dualPistolasAchievement).Key
//                      //AchievementIds.Values.ToList().IndexOf(dualPistolasAchievement)
//                      + " ======= " + incrementValue);

			((PlayGamesPlatform) Social.Active).IncrementAchievement(
				dualPistolasAchievement.AchievementId, incrementValue, (bool success) => {
			});
        }
		else
        {
            if(m_UnlockedAchievements.ContainsKey(dualPistolasAchievement.AchievementId) == false)
            {
//                Debug.Log(".......................................................Unlock " + 
//                          //dualPistolasAchievement.AchievementId 
//                          AchievementIds.FirstOrDefault(x => x.Value == dualPistolasAchievement).Key
//                          + " ==== UNLOCK!");

                m_UnlockedAchievements[dualPistolasAchievement.AchievementId] = true;
    			Social.ReportProgress(dualPistolasAchievement.AchievementId, 100.0, ((bool success) => {}));
            }
        
        }
		#endif
	}

	/// <summary>
	/// Adds the achievement to our local cache to allow us to increment properly.
	/// </summary>
	void AddAchievementToCache(DualPistolasAchievement dualPistolasAchievement, double progress)
	{
		IAchievement tempAchievement = Social.CreateAchievement();
        tempAchievement.id = dualPistolasAchievement.AchievementId;
		tempAchievement.percentCompleted = progress;

		List<IAchievement> tempList = new List<IAchievement>();
		foreach (IAchievement achievement in m_Achievements)
		{
			tempList.Add(achievement);
		}
		tempList.Add(tempAchievement);

		m_Achievements = tempList.ToArray();
	}

	
}
