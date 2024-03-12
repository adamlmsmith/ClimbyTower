public class DualPistolasAchievement {

	public int NumSteps { get; set; }
	public string AchievementId { get; set; }

	public static DualPistolasAchievement Create(int numSteps, string achievementId)
	{
		DualPistolasAchievement achievement = new DualPistolasAchievement();
		achievement.NumSteps = numSteps;
		achievement.AchievementId = achievementId;
		return achievement;
	}

	public DualPistolasAchievement()
	{
	}
}
