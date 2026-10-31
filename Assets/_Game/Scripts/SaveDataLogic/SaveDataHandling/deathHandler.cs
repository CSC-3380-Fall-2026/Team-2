using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;

//Death Handler that manages death and properly restoring to previous checkpoints.
//Additional UI implementations will be added later.
public class deathHandler : MonoBehaviour
{
    //Death Delay currently set to 3 seconds before respawning (3f).
    [SerializeField, Min(0f)] private float deathDelay = 3f;

    private bool isHandlingDeath; //default = false

    //Method checks if death is already being handled & saveDataHandler exist.
    //If true, then starts handling death loading.
    public bool handleDeath(Action onFinished = null) {
        if (isHandlingDeath) {
            return false;
        }
        if (saveDataHandler.instance == null) {
            Debug.LogError("Cannot handle death as no save data handler exists. deathHandler.cs", this);
            return false;
        }
        
        isHandlingDeath = true;
        StartCoroutine(loadAfterDeath(onFinished));
        return true;
    }

    //Method that does all the handling, restores the player
    //to the previous checkpoint after finishing.
    private IEnumerator loadAfterDeath(Action onFinished) {
        yield return new WaitForSecondsRealtime(deathDelay);

        try {
            if (saveDataHandler.instance == null) {
                Debug.LogError("Cannot load as no save data handler exists. deathHandler.cs", this);
            }
            else {
                saveDataHandler.instance.loadFile();
            }
        }
        finally {
            isHandlingDeath = false;
            onFinished?.Invoke();
        }
    }
}