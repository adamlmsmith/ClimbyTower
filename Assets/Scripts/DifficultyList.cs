using UnityEngine;
//using System;
using System.Collections.Generic;
using UnityEngine.Serialization;
public class DifficultyList : MonoBehaviour
{
    public AnimationCurve NumClosingWindows;
    public AnimationCurve NumDroppers;
    public AnimationCurve NumShooters;
    public AnimationCurve BiplaneFrequency;
    public AnimationCurve ChanceOfPowerLine;
    public AnimationCurve ChanceOfProximityMine;

    //This is our custom class with our variables
    [System.Serializable]
    public class MyClass{
        public int StartingFloorNumber;
        public List<FloorGroup> FloorGroups;

        public int NextFloorGroupIndex = 0;

//        public GameObject AnGO;
//        public int AnInt;
//        public float AnFloat;
//        public Vector3 AnVector3;
//        public int[] AnIntArray = new int[0];
    }
        
    //This is our list we want to use to represent our class as an array.
    public List<MyClass> MyList = new List<MyClass>(1);


    void Start()
    {
        for(int i = 0; i < MyList.Count; i++)
        {
            ShuffleFloorGroups(i);
        }
    }

    void AddNew(){
        //Add a new index position to the end of our list
        MyList.Add(new MyClass());
    }
    
    void Remove(int index){
        //Remove an index position from our list at a point in our list array
        MyList.RemoveAt(index);
    }

    public int GetFloorDifficulty(int floorNumber)
    {        
        int difficulty = 0;
        
        // Find the current difficulty based on the highest floor reached by the climber
        while((difficulty + 1 <=  GameVariables.instance.DifficultyList.MyList.Count - 1) &&
              (floorNumber >= GameVariables.instance.DifficultyList.MyList[difficulty + 1].StartingFloorNumber))
        {
            difficulty++;
        }

        return difficulty;
    }

    void ShuffleFloorGroups(int difficulty)
    {
        int randomIndex;
        
        for (int i = 0; i < MyList[difficulty].FloorGroups.Count; i++)
        {
            FloorGroup floorGroupToSwap = MyList[difficulty].FloorGroups[i];
            randomIndex = Random.Range(0, MyList[difficulty].FloorGroups.Count);
            MyList[difficulty].FloorGroups[i] = MyList[difficulty].FloorGroups[randomIndex];
            MyList[difficulty].FloorGroups[randomIndex] = floorGroupToSwap;
        }

        MyList[difficulty].NextFloorGroupIndex = 0;
    }

    public FloorGroup GetNextFloorGroup(int difficulty)
    {
        Debug.Assert(difficulty >= 0 && difficulty < MyList.Count);

        int floorGroupsCount = MyList[difficulty].FloorGroups.Count;

        Debug.Assert(floorGroupsCount > 0);

        if (MyList [difficulty].NextFloorGroupIndex == MyList[difficulty].FloorGroups.Count)
        {
            ShuffleFloorGroups(difficulty);
        }

        int returnFloorGroupIndex = MyList [difficulty].NextFloorGroupIndex;

        MyList [difficulty].NextFloorGroupIndex++;

        return MyList[difficulty].FloorGroups[returnFloorGroupIndex];
    }
}