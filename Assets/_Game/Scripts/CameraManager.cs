using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{
    public Player playerScript;
    GameObject playerObj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /*void Start()
    {
        playerScript = Object.FindAnyObjectByType<Player>(); 
        playerScript.getCameraBounds();

    }

    // Update is called once per frame
    void Update()
    {
        
    }*/


    private void OnEnable()
    {
        // Subscribe to the sceneLoaded event when the script becomes active
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Always unsubscribe to prevent memory leaks and duplicate execution
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // This method runs automatically every time a new scene is finished loading
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");
        
        // Put the code you want to run every time here:
        //ResetLevelVariables();
        playerObj = GameObject.FindWithTag("Player");
        playerScript = playerObj.GetComponent<Player>();
        playerScript.getCameraBounds();
    }

    private void ResetLevelVariables()
    {
        // Your logic here
    }
}