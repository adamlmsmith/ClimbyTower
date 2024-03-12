using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClimberManager : MonoBehaviour
{
    [SerializeField]
    List<Climber> climberPrefabs;

    int selectedClimberIndex;
    int randomClimberIndex = 0;

    public int SelectedClimberIndex
    { 
        get
        {
            return selectedClimberIndex;
        } 
        set
        {
            selectedClimberIndex = Mathf.Clamp(value, 0, climberPrefabs.Count - 1);
            GameManager.instance.Progress.SelectedClimber = climberPrefabs[SelectedClimberIndex].name;
        }
    }

    public int CurrentClimberIndex
    {
        get
        {
            if(IsRandomClimber(selectedClimberIndex))
            {
                return GetRandomUnlockedClimberIndex();
            }
            
            return selectedClimberIndex;
        }
    }

    void Awake()
    {
        GameManager.instance.UnlockCostume("Random");
        GameManager.instance.UnlockCostume("Gorilla");

        string selectedClimber = GameManager.instance.Progress.SelectedClimber;

        if (selectedClimber == null)
            SelectedClimberIndex = 1;
        else
        {
            SelectedClimberIndex = climberPrefabs.FindIndex(x => x.name == selectedClimber);
        }

        for(int i = 0; i < climberPrefabs.Count; i++)
        {
            climberPrefabs[i].Unlocked = IsClimberUnlocked(climberPrefabs[i].ClimberName);
        }
    }
        
#if UNITY_EDITOR
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            for(int i = 0; i < climberPrefabs.Count; i++)
            {
                UnlockClimber(climberPrefabs[i].ClimberName);
            }
        }
    }
#endif

    public bool IsClimberUnlocked(string climberName)
    {
        return(GameManager.instance.Progress.IsCostumeUnlocked(climberName));
    }

    public void UnlockClimber(string climberName)
    {
        climberPrefabs[GetClimberIndexForName(climberName)].Unlocked = true;
        GameManager.instance.UnlockCostume(climberName);
    }

    public int GetClimberCount()
    {
        return climberPrefabs.Count;
    }

    public Climber GetClimber(int climberIndex)
    {
        Debug.Assert(climberIndex >= 0 && climberIndex < climberPrefabs.Count);
        climberIndex = Mathf.Clamp(climberIndex, 0, climberPrefabs.Count - 1);
        return climberPrefabs [climberIndex];
    }

    public Climber GetCurrentClimber()
    {
        return climberPrefabs[CurrentClimberIndex];
    }

    public bool IsRandomClimber(int climberIndex)
    {
        return(climberIndex == randomClimberIndex);
    }

    public int GetRandomClimberIndex()
    {
        return Random.Range(1, climberPrefabs.Count);
    }

    public int GetRandomUnlockedClimberIndex()
    {
        bool done = false;
        int randomIndex = 0;

        while (!done)
        {
            randomIndex = Random.Range(1, climberPrefabs.Count);

            if (climberPrefabs [randomIndex].Unlocked == true)
                done = true;
        }

        return randomIndex;
    }

    public int GetClimberIndexForName(string climberName)
    {
        int climberIndex = 0;

        for (int i = 0; i < climberPrefabs.Count; i++)
        {
            if(climberPrefabs[i].ClimberName == climberName)
                climberIndex = i;
        }

        return climberIndex;
    }

    public Climber PickClimberForPrize()
    {
        float climberRarity = Random.Range(0.0f, 1.0f);
        Climber.Rarity rarity = Climber.Rarity.COMMON;

        if (climberRarity <= 0.8f)
        {
            rarity = Climber.Rarity.COMMON;
        }
        else if (climberRarity <= 0.91f)
        {
            rarity = Climber.Rarity.RARE;
        }
        else if (climberRarity <= 0.97f)
        {
            rarity = Climber.Rarity.EPIC;
        }
        else
        {
            rarity = Climber.Rarity.LEGENDARY;
        }

        List<Climber> climbers = new List<Climber>();
        
        climbers.AddRange(climberPrefabs.FindAll(c => c.ClimberRarity == rarity));

        if (climbers.Count > 0)
        {
            int randomIndex = Random.Range(0, climbers.Count);
            return climbers[randomIndex];
        }
        
        return null;
    }

    public bool AllClimbersUnlocked()
    {
        return (GetUnlockedClimberCount() == climberPrefabs.Count);
    }

    public int GetUnlockedClimberCount()
    {
        int unlockedClimberCount = 0;

        for(int i = 0; i < climberPrefabs.Count; i++)
        {
            if(climberPrefabs[i].Unlocked)
            {
                unlockedClimberCount++;
            }
        }

        return unlockedClimberCount;
    }

    public int GetLockedClimberCount()
    {
        int lockedClimberCount = 0;
        
        for(int i = 0; i < climberPrefabs.Count; i++)
        {
            if(climberPrefabs[i].Unlocked == false)
            {
                lockedClimberCount++;
            }
        }
        
        return lockedClimberCount;
    }
}
