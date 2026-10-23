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
    public List<Vector2> roomCenters = new List<Vector2>();
    public List<Vector2> roomBounds = new List<Vector2>();
    public bool isPuzzleSolved = false;
    public string nextScene = null; //leave null unless the next room should load another scene
    
    public SceneData leftRoom;
    public SceneData rightRoom;
    public SceneData upRoom;
    public SceneData downRoom;
    
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
        byte enemyId = 0;
        if(enemyNameList.Contains(enemyName))
        {
            enemyId = enemyNameList.IndexOf(enemyName);//caps at 255 (should be plenty enough)
        }
        isEnemyAliveList[enemyId] = false;
    }

}

