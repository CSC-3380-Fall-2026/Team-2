using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewRoom", menuName = "Save System/Room")]
public class RoomData : ScriptableObject
{
    [Header("Room Information")]
    public string roomID;
    public string currentScene;

    public Vector2 roomCenter;
    public Vector2 roomBounds;

    [Header("Room Connections")]
    public RoomData leftRoom;
    public RoomData rightRoom;
    public RoomData upRoom;
    public RoomData downRoom;

    [Header("Default Room State")]
    public bool isPuzzleSolved = false;

    public List<bool> isEnemyAliveList = new();
    public List<bool> isEnemyAbleRespawnList = new();
    public List<Vector2> enemyLocationList = new();
    public List<string> enemyNameList = new();
}