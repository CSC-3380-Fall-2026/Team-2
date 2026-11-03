using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Save Data, variables that need to be saved are initialized in the
//public class, their values are then set in the public method.
//Additional changes will be needed down the line to deal with diff
//things that need saving, but for now this works fine.

[System.Serializable]
public class SceneData {
    //public int placeholder; <- Create var like this.
    public Vector2 roomCenter;
    public Vector2 roomBounds;
    public bool isPuzzleSolved = false;
    public string currentScene = null; //when moving to the next room, check if the Scene is the same
    
    [SerializeReference] public SceneData leftRoom;
    [SerializeReference] public SceneData rightRoom;
    [SerializeReference] public SceneData upRoom;
    [SerializeReference] public SceneData downRoom;
    
    public List<bool> isEnemyAliveList = new List<bool>();
    public List<bool> isEnemyAbleRespawnList = new List<bool>(); 
    public List<Vector2> enemeyLocationList = new List<Vector2>();
    public List<string> enemyNameList = new List<string>();

    public SceneData() 
    {
        
        //this.placeholder = 0; <- Assign default here.
    }

    void onEnemyDeath(string enemyName)
    {
        int enemyId = 0;
        if(enemyNameList.Contains(enemyName))
        {
            enemyId = enemyNameList.IndexOf(enemyName);//caps at 255 (should be plenty enough)
        }
        isEnemyAliveList[enemyId] = false;
    }

}

