using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public PlayerState currentState;
    private Coroutine speedResetCoroutine;
    
    [Header("Phase Settings")]
    public float phaseThreshold = 15f;
    public LayerMask phaseLayers;
    
    [Header("Bounce Settings")]
    public PhysicsMaterial2D bounceMaterial;
    public float bounceSpeedMultiplier = 2f;
    public float bounceSpeedDuration = 1f;
    public float maxBounceSpeed = 60f;
    
    [Header("Spin Settings")]
    public float currentCharge;
    public float chargePerMash = 4f;
    public float maxCharge = 30f;
    public float spinDecayRate = 2f;
    public float spinDeceleration = 1f;
    public bool isCameraBound = true;
    public bool isSpinning;
    public bool isLaunching;
    

    [Header("Components")]
    public Rigidbody2D rb;
    public CircleCollider2D playerCollider;
    public GameObject playerObject;
    private static Player instance;
    public GameObject cameraBounds;
    public Vector2 cameraCenter;
    
    [Header("Movement Settings")]
    [SerializeField] public float acceleration = 0.1f;
    [SerializeField] public float deceleration = 0.2f;
    [SerializeField] public float maxSpeed = 20f;
    
    public Vector2 direction;

    void Start()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(playerObject);
        }
        else 
        {
            Destroy(playerObject);
        }
        ChangeState(new PlayerIdleState(this));
        getCameraBounds();
    }

    void Update()
    {
        currentState.Update();
        HandlePhasing();
    }

    void FixedUpdate()
    {
        currentState.PhysicsUpdate();
        checkRoom();
    }

    public void ChangeState(PlayerState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        currentState = newState;
        currentState.Enter();
    }

    void OnMove(InputValue move)
    {
        direction = move.Get<Vector2>();
    }

    void OnSpin(InputValue spin)
    {
        isSpinning = spin.isPressed;
        if (isSpinning && !(currentState is PlayerSpinState))
        {
            currentCharge = 0f;
            ChangeState(new PlayerSpinState(this));
        }

        if (currentState is PlayerSpinState spinState)
        {
            spinState.HandleSpinInput(isSpinning);
        }
    }

    void OnShoot(InputValue shoot)
    {
        if (shoot != null)
        {
            isLaunching = true;
        }
    }

    void HandlePhasing()
    {
        bool canPhase = rb.linearVelocity.magnitude >= phaseThreshold;
        if (canPhase)
        {
            playerCollider.excludeLayers = phaseLayers;
        }
        else if (playerCollider.excludeLayers == phaseLayers)
        {
            Vector2 checkPosition = (Vector2)transform.position + playerCollider.offset;
            Collider2D insideWall = Physics2D.OverlapCircle(checkPosition, playerCollider.radius, phaseLayers);
            if (insideWall == null)
            {
                playerCollider.excludeLayers = 0;
            }
        }
    }
    
    //For the Player Bounce, The player must collide with a surface that has the same bounce material
    void OnCollisionEnter2D(Collision2D bounce)
    {
        if (bounce.collider.sharedMaterial != bounceMaterial)
        {
            return;
        }

        maxSpeed = Mathf.Min(bounce.relativeVelocity.magnitude * bounceSpeedMultiplier, maxBounceSpeed);

        if (speedResetCoroutine != null)
        {
            StopCoroutine(speedResetCoroutine);
        }

        speedResetCoroutine = StartCoroutine(ResetMaxSpeed());
    }
    
    IEnumerator ResetMaxSpeed()
    {
        yield return new WaitForSeconds(bounceSpeedDuration);
        maxSpeed = 20f;
    }


    void OnUnBindCamera() // for debuggin mostly
    {
        CenterCamera();
        isCameraBound = !isCameraBound;
        Debug.Log(isCameraBound);
    }

    void CenterCamera()
    {
            cameraBounds.transform.position = cameraCenter;
    }

    void checkRoom ()
    {
        if (!isCameraBound)
        {
            cameraBounds.transform.position = playerObject.transform.position;
        }
        if (gameObject.transform.position.x <= cameraBounds.transform.position.x -18)
        {
            EnterRoom(Vector2.left);
            //OnUnBindCamera();
        }
        if (gameObject.transform.position.x >= cameraBounds.transform.position.x +18)
        {
            EnterRoom(Vector2.right);
            //OnUnBindCamera();
        }
        if (gameObject.transform.position.y <= cameraBounds.transform.position.y -10)
        {
            EnterRoom(Vector2.down);
            //OnUnBindCamera();
        }
        if (gameObject.transform.position.y >= cameraBounds.transform.position.y +10)
        {
            EnterRoom(Vector2.up);
            //OnUnBindCamera();
        }
    }

    void EnterRoom(Vector2 roomDirection)
    {
        //temporary solution, later we will get the direct point to the enemies from save file
        Debug.Log("Enter new room");
        GameObject parentObj = GameObject.Find("leftRoom");;
        Scene currentScene = SceneManager.GetActiveScene();
        if (roomDirection == Vector2.left)
        {
            //parentObj = GameObject.Find("leftRoom");
            cameraCenter.x = cameraCenter.x -36;
        }
        if (roomDirection == Vector2.right)
        {
            //parentObj = GameObject.Find("rightRoom");
            cameraCenter.x = cameraCenter.x + 36;
        }
        if (roomDirection == Vector2.up)
        {
            //parentObj = GameObject.Find("upRoom");
            cameraCenter.y = cameraCenter.y + 20;
            if (currentScene.name == "Game") //the following lines should reside in a statement that checks if the next room is in another scene
            {   
                SceneManager.LoadScene("Game 2");
                Vector2 tempPosition;
                tempPosition.x = gameObject.transform.position.x%20;
                tempPosition.y = gameObject.transform.position.y%10;
                gameObject.transform.position = tempPosition;
                //gameObject.transform.position = gameObject.transform.position - new Vector2(20*(gameObject.transform.position.x/20),10*(gameObject.transform.position.y/10));
            }
            cameraBounds = GameObject.Find("CameraBounds");
        }
        if (roomDirection == Vector2.down)
        {
            //parentObj = GameObject.Find("downRoom");
            if (currentScene.name == "Game 2")
            {SceneManager.LoadScene("Game");}
            cameraCenter.y = cameraCenter.y - 20;
        }
        CenterCamera();

        if (parentObj != null)
        {
            // 2. Search the parent's children (works even if "Secret_Door" is disabled)
            Transform childTransform = parentObj.transform.Find("example enemy");
            
            if (childTransform != null)
            {
                GameObject exampleEnemy = childTransform.gameObject;
                // You can now activate it:
                exampleEnemy.SetActive(true);
            }
        }
    }

    public void getCameraBounds ()
    {
        cameraBounds = GameObject.Find("CameraBounds");
        cameraCenter = cameraBounds.transform.position;
    }

}
