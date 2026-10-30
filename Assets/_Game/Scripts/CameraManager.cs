using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{
    public Player playerScript;
    GameObject playerObj;
    public SceneData sceneData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerScript = Object.FindAnyObjectByType<Player>(); 
        playerScript.getCameraBounds();

    }

    // Update is called once per frame
    void Update()
    {
        
    }


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


    public void EnterRoom(Vector2 roomDirection, Vector2 cameraCenter, Vector2 cameraBounds)
    {
        SceneData nextRoomData = null;
        //temporary solution, later we will get the direct point to the enemies from save file
        Debug.Log("Enter new room");
        //Scene currentScene = SceneManager.GetActiveScene();
        
        if (roomDirection == Vector2.left)
        {
            cameraCenter.x = cameraCenter.x -36;
            nextRoomData = sceneData.leftRoom;
        }
        if (roomDirection == Vector2.right)
        {
            cameraCenter.x = cameraCenter.x + 36;
            nextRoomData = sceneData.rightRoom;
        }
        if (roomDirection == Vector2.up)
        {
            cameraCenter.y = cameraCenter.y + 20;
            nextRoomData = sceneData.upRoom;
        }
        if (roomDirection == Vector2.down)
        {
            cameraCenter.y = cameraCenter.y - 20;
            nextRoomData = sceneData.downRoom;
        }
        if (sceneData.currentScene!=nextRoomData.currentScene)
        {
            SceneManager.LoadScene(nextRoomData.currentScene);
        }
        sceneData = nextRoomData;
        cameraCenter = sceneData.roomCenter;
        cameraBounds = sceneData.roomBounds;
        playerScript.CenterCamera(cameraCenter, cameraBounds);
    }
}