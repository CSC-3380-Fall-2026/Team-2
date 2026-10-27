using System.Collections;
using UnityEngine;

//Death Handler that manages death and properly restoring to previous checkpoints.
//Additional UI implementations will be added later.
public class deathHandler : MonoBehaviour
{
    //Death Delay currently set to 3 seconds before respawning (3f).
    [SerializeField, Min(0f)] private float deathDelay = 3f;

    private bool isHandlingDeath; //default = false

    //Method checks if death is being handled, true meaning return, false meaning start handling.
    public void HandleDeath() {
        if (isHandlingDeath) {
            return;
        }
        isHandlingDeath = true;
        StartCoroutine(LoadAfterDeath());
    }

    //Method waits for defined delay time (yield), then checks for a saveHandler, before loading file. 
    private IEnumerator LoadAfterDeath() {
        yield return new WaitForSecondsRealtime(deathDelay);
        if (saveDataHandler.instance == null) {
            Debug.LogError("Cannot load: no save data handler exists.", this);
            isHandlingDeath = false;
            yield break;
        }
        saveDataHandler.instance.loadFile();
        isHandlingDeath = false;
    }
}