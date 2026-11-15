using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//Checkpoint Handler that manages saving data when the player interacts with a checkpoint (via pressing O).
//Additional implementations like the UI will also eventually be added here.
public class checkpointHandler : MonoBehaviour {
    
    //intRange = interaction range for checkpoints (currently 12 tiles).
    [SerializeField, Min(0f)] private float intRange = 4f;
    
    //Method verifies if the user has pressed the O key. 
    private void Update() {
        Keyboard keyboard = Keyboard.current;

        if (keyboard != null && !keyboard.ctrlKey.isPressed && keyboard.oKey.wasPressedThisFrame) {
            interactWithCheckpoint();
        }

        Player player = GetComponent<Player>();
        
        if (player == null || player.isDead) {
            return;
        }
        if (saveDataHandler.instance != null && saveDataHandler.instance.isLoading) {
            return;
        }
    }

    //Checks for nearest checkpoint, if none is in range or no saveHandler, return, otherwise saves.
    public void interactWithCheckpoint() {
        Player player = GetComponent<Player>();
        checkpointIdent checkpoint = findNearestCheckpoint();

        if (player == null || player.isDead) {
            return;
        }

        if (checkpoint == null) {
            return;
        }

        if (saveDataHandler.instance == null) {
            Debug.LogError("Cannot save: no SaveHandler exists. checkpointHandler.cs", this);
            return;
        }

        saveDataHandler.instance.saveCheckpoint(checkpoint);
        Debug.Log("Save Called.");
    }

    //Searches for nearest checkpoint in range, via math calculations.
    private checkpointIdent findNearestCheckpoint() {
        checkpointIdent nearestCheckpoint = null;
        float nearestDistanceSquared = intRange * intRange;
        checkpointIdent[] checkpoints = FindObjectsByType<checkpointIdent>();

        foreach (checkpointIdent checkpoint in checkpoints) {
            if (!checkpoint.isActiveAndEnabled) {
                continue;
            }

            if (checkpoint.gameObject.scene != SceneManager.GetActiveScene()) {
                continue;
            }

            Vector2 offset = (Vector2)checkpoint.transform.position - (Vector2)transform.position;
            float distanceSquared = offset.sqrMagnitude;

            if (distanceSquared <= nearestDistanceSquared) {
                nearestDistanceSquared = distanceSquared;
                nearestCheckpoint = checkpoint;
            }
        }

        return nearestCheckpoint;
    }

}