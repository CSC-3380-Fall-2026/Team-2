using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    GameObject playerObj;
    public Player playerScript;
    public SceneData sceneData;
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
            }
        }

        SetPlayer(playerScript);
    }

    public void SetPlayer(Player player)
    {
        playerScript = player;
        player.cameraManager = this;
    }

    private void ResetLevelVariables()
    {
        // Your logic here
    }


    public void EnterRoom(Vector2 roomDirection)
    {
        //SceneData nextRoomData = null;
        //temporary solution, later we will get the direct point to the enemies from save file
        //Debug.Log("Enter new room");
        //Scene currentScene = SceneManager.GetActiveScene();
        
        if (roomDirection == Vector2.left)
        {
            cameraCenter.x = cameraCenter.x -36;
            //Debug.Log(cameraCenter);
            //nextRoomData = sceneData.leftRoom;
        }
        if (roomDirection == Vector2.right)
        {
            cameraCenter.x = cameraCenter.x + 36;
            //Debug.Log(cameraCenter);
            //nextRoomData = sceneData.rightRoom;
        }
        if (roomDirection == Vector2.up)
        {
            cameraCenter.y = cameraCenter.y + 20;
            //Debug.Log(cameraCenter);
            //nextRoomData = sceneData.upRoom;
        }
        if (roomDirection == Vector2.down)
        {
            cameraCenter.y = cameraCenter.y - 20;
            //Debug.Log(cameraCenter);
            //nextRoomData = sceneData.downRoom;
        }
        /*if (sceneData.currentScene!=nextRoomData.currentScene)
        {
            SceneManager.LoadScene(nextRoomData.currentScene);
        }*/
        //sceneData = nextRoomData;
        //cameraCenter = sceneData.roomCenter;
        //cameraBounds = sceneData.roomBounds;
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