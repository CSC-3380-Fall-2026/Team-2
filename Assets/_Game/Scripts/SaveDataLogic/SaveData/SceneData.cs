using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SceneData
{
    public string roomID;

    public bool isPuzzleSolved;

    public List<bool> isEnemyAliveList = new();
    public List<bool> isEnemyAbleRespawnList = new();
    public List<Vector2> enemyLocationList = new();
    public List<string> enemyNameList = new();
}