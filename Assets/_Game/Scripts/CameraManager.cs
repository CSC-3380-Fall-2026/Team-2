using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    GameObject playerObj;
    public Player playerScript;
    public SceneData sceneData;
    public RoomData roomData;
    [SerializeField] public GameObject cameraBounds;
    [SerializeField] public Vector2 cameraCenter;
    public bool isCameraBound = true;
    [SerializeField] private CinemachineConfiner2D confiner;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(FindPlayer());

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
        StartCoroutine(FindPlayer());
    }

    private IEnumerator FindPlayer()
    {
        while (playerScript == null)
        {
            playerObj = GameObject.FindWithTag("Player");

            if (playerObj != null)
            {
                playerScript = playerObj.GetComponent<Player>();
            }

            if (playerScript == null)
            {
                yield return null;
                Debug.Log("null");
            }
        }

        SetPlayer(playerScript);
    }

    public void SetPlayer(Player player)
    {
        playerScript = player;
        player.cameraManager = this;
        player.cameraBounds = this.cameraBounds;
    }

    private void ResetLevelVariables()
    {
        // Your logic here
    }


    public void EnterRoom(Vector2 roomDirection)
    {
        RoomData nextRoomData = null;
        
        Scene currentScene = SceneManager.GetActiveScene();
        
        if (roomDirection == Vector2.left)
        {
            cameraCenter.x = cameraCenter.x -38;
            //Debug.Log(cameraCenter);
            nextRoomData = roomData.leftRoom;
        }
        if (roomDirection == Vector2.right)
        {
            cameraCenter.x = cameraCenter.x + 38;
            //Debug.Log(cameraCenter);
            nextRoomData = roomData.rightRoom;
        }
        if (roomDirection == Vector2.up)
        {
            cameraCenter.y = cameraCenter.y + 20;
            //Debug.Log(cameraCenter);
            nextRoomData = roomData.upRoom;
        }
        if (roomDirection == Vector2.down)
        {
            cameraCenter.y = cameraCenter.y - 20;
            //Debug.Log(cameraCenter);
            nextRoomData = roomData.downRoom;
        }
        if (nextRoomData == null)
        {   
            CenterCamera(cameraCenter, cameraBounds.transform.localScale); //camera operates under normal room by room checking
            return;
        }

        //CaptureCurrentRoomState();    

        
        if (roomData.currentScene!=nextRoomData.currentScene)
        {
            //SceneManager.LoadScene("Game2");
            //playerScript.gameObject.transform.position = new Vector3(playerScript.gameObject.transform.position.x - cameraCenter.x, playerScript.gameObject.transform.position.y - cameraCenter.y, playerScript.gameObject.transform.position.z);
            Vector3 changedPosition = new Vector3(roomDirection.x*(roomData.roomBounds.x/2-3)*-1,roomDirection.y*(roomData.roomBounds.y/2-3)*-1,0);
            playerScript.gameObject.transform.position = changedPosition;

            //playerScript.gameObject.transform.position.y = playerScript.gameObject.transform.position.y - roomData.roomCenter.y
            SceneManager.LoadScene(nextRoomData.currentScene);
        }
        

        roomData = nextRoomData;

        //LoadCurrentRoomState();

        cameraCenter = roomData.roomCenter;
        cameraBounds.transform.localScale = roomData.roomBounds;

        
        CenterCamera(cameraCenter, cameraBounds.transform.localScale);
    }

    public void CenterCamera(Vector2 center, Vector2 bounds)
    {
        if (cameraBounds == null)
            return;

        cameraCenter = center;

        cameraBounds.transform.position =
            new Vector3(center.x, center.y, cameraBounds.transform.position.z);

        cameraBounds.transform.localScale =
            new Vector3(bounds.x, bounds.y, 1f);

        Physics2D.SyncTransforms();

        if (confiner != null)
        {
            confiner.InvalidateBoundingShapeCache();
        }

        Debug.Log($"Camera center updated: {cameraCenter}");
    }

    public void ToggleCameraBound()
    {
        CenterCamera(cameraCenter, cameraBounds.transform.localScale);
        isCameraBound = !isCameraBound;
        Debug.Log(isCameraBound);
    }
}