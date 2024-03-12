using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameVariables : MonoBehaviour
{
	public bool EasyWin = false;

    public bool SkipStartScreenInEditor = true;

    public int CoinsPerPrize = 100;

    public int MinutesBetweenGifts = 360;

    public int FloorsPerGift = 600;

    public int MaxNumPlayers = 1;

    public int MaxNumRounds = 1;

    public static GameVariables instance;

    void Awake()
    {
        instance = this;

        MaxNumPlayers = 1;
    }

    public Game Game;
    public PlayerManager PlayerManager;
    public BoardHolder BoardHolder;
    public LogoScreen LogoScreen;
    public StartScreen StartScreen;
    public DifficultyList DifficultyList;
    public HUD HUD;
    public TallyScreen TallyScreen;
    public GiftScreen GiftScreen;
    public PauseScreen PauseScreen;
    public ClimberSelectScreen ClimberSelectScreen;
    public ClimberManager ClimberManager;
    public PrizeScreen PrizeScreen;
    public PlayerInputArea PlayerInputArea;
    public QuestManager QuestManager;
    public GiftManager GiftManager;
    public CreditsScreen CreditsScreen;
    public SignInScreen SignInScreen;

    [HideInInspector]
    public Board Board;
}
