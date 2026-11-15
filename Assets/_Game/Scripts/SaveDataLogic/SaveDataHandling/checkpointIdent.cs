using UnityEngine;

//This script is attached to the checkpoint object, which 
//assists in handling important checkpoint information.
public class checkpointIdent : MonoBehaviour {

//Variables
public string checkpointId;
public Transform spawn;
public RoomData roomData;
public CameraManager roomCamera;

//Checks if the checkpoint has all the required information, false if any is missing.
public bool isConfigured() {
    return !string.IsNullOrWhiteSpace(checkpointId) && spawn != null && roomData != null && roomCamera != null;
}

//Restores checkpoint room, camera reference, and centers the camera to the assigned room.
public void respawn(Player player) {
    roomCamera.roomData = roomData;
    roomCamera.SetPlayer(player);
    roomCamera.CenterCamera(roomData.roomCenter, roomData.roomBounds);
    player.restoreCheckpointPos(spawn.position);
}

}