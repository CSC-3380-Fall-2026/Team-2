using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//Checkpoint Handler that manages saving data when the player interacts with a checkpoint (via pressing O).
//Additional implementations like the UI will also eventually be added here.
public class CheckPointHandler : MonoBehaviour, iSaveData {
    
    //intRange = interaction range for checkpoints (currently 12 tiles).
    //checkpoints = array of checkpoint references.
    [SerializeField, Min(0f)] private float intRange = 4f;
    [SerializeField] private Transform[] checkpoints = new Transform[0];

    //Method verifies if the user has pressed the O key. 
    private void Update() {
        Keyboard keyboard = Keyboard.current;

        if (keyboard != null && !keyboard.ctrlKey.isPressed && keyboard.oKey.wasPressedThisFrame) {
            InteractWithCheckpoint();
        }
    }

    //Starts checking for refreshes on scene load.
    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
        refreshCheckpoints();
    }

    //Stops checking on destruction.
    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //Refreshs checkpoint array on request after scene load.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        refreshCheckpoints();
    }

    //Clears old array, searches loaded scene for checkpoint objects, loads them into array.
    private void refreshCheckpoints() {
        checkpoints = new Transform[0];
        GameObject checkpoint = GameObject.Find("goldsavebrightshine1_0"); //Change this if checkpoint sprite name changes.
        if (checkpoint == null) {
            return;
        }

        checkpoints = new Transform[] {checkpoint.transform}; 
        Debug.Log("Checkpoints Refreshed.");
    }

    //Checks for nearest checkpoint, if none is in range or no saveHandler, return, otherwise saves.
    public void InteractWithCheckpoint() {
        Transform checkpoint = FindNearestCheckpoint();

        if (checkpoint == null) {
            return;
        }

        if (saveDataHandler.instance == null) {
            Debug.LogError("Cannot save: no SaveHandler exists. checkpointHandler.cs", this);
            return;
        }

        //Checkpoint data goes here!
        saveDataHandler.instance.saveFile();
    }

    //Searches for nearest checkpoint in range, via math calculations.
    private Transform FindNearestCheckpoint() {
        Transform nearestCheckpoint = null;
        float nearestDistanceSquared = intRange * intRange;

        foreach (Transform checkpoint in checkpoints) {
            if (checkpoint == null) {
                continue;
            }

            Vector2 offset = (Vector2)checkpoint.position - (Vector2)transform.position;
            float distanceSquared = offset.sqrMagnitude;

            if (distanceSquared <= nearestDistanceSquared) {
                nearestDistanceSquared = distanceSquared;
                nearestCheckpoint = checkpoint;
            }
        }

        return nearestCheckpoint;
    }

    //Called to restore data if necessary.
    public void loadData(SaveData data) {
        //Checkpoint Data goes here!
    }

    //Called when collection info to save.
    public void saveData(ref SaveData data) {
        //Checkpoint Data goes here!
    }
}