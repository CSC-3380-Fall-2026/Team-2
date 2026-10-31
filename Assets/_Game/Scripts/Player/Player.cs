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
    
    public bool isSpinning;
    public bool isLaunching;
    

    [Header("Components")]
    public Rigidbody2D rb;
    public CircleCollider2D playerCollider;
    public GameObject playerObject;
    [SerializeField] private static Player instance;
    
    [SerializeField] public CameraManager cameraManager;
    
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
            gameObject.tag = "Player";
        }
        else 
        {
            Debug.Log($"Destroyed Clone");
            Destroy(playerObject);
        }
        ChangeState(new PlayerIdleState(this));
        //getCameraBounds();
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
            if (cameraManager != null)
        {
            cameraManager.ToggleCameraBound();
        }
    }

    void checkRoom ()
    {
        if (cameraManager == null)
        {return;} //ensures we have everything loaded before checking rooms

        if (!cameraManager.isCameraBound)
        {
            cameraManager.cameraBounds.transform.position = playerObject.transform.position;
        }
        if (gameObject.transform.position.x <= cameraManager.cameraBounds.transform.position.x -18)
        {
            cameraManager.EnterRoom(Vector2.left);
            Debug.Log("Enter left");
        }
        else if (gameObject.transform.position.x >= cameraManager.cameraBounds.transform.position.x +18)
        {
            cameraManager.EnterRoom(Vector2.right);
            Debug.Log("Enter right");
        }
        else if (gameObject.transform.position.y <= cameraManager.cameraBounds.transform.position.y -10)
        {
            cameraManager.EnterRoom(Vector2.down);
            Debug.Log("Enter down");
        }
        else if (gameObject.transform.position.y >= cameraManager.cameraBounds.transform.position.y +10)
        {
            cameraManager.EnterRoom(Vector2.up);
            Debug.Log("Enter up");
        }
    }

    /*public void getCameraBounds ()
    {
        cameraBounds = GameObject.Find("CameraBounds");
        cameraManager = cameraBounds.GetComponent<CameraManager>();
        cameraCenter = cameraBounds.transform.position;
        CenterCamera(cameraCenter, cameraBounds.transform.localScale);
    }*/

}
