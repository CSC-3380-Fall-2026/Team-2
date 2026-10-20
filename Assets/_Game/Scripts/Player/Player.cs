using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public GameObject cameraBounds;
    public Vector2 cameraCenter;
    
    [Header("Movement Settings")]
    [SerializeField] public float acceleration = 0.1f;
    [SerializeField] public float deceleration = 0.2f;
    [SerializeField] public float maxSpeed = 20f;
    
    public Vector2 direction;

    void Start()
    {
        ChangeState(new PlayerIdleState(this));
        cameraCenter = cameraBounds.transform.position;
    }

    void Update()
    {
        currentState.Update();
        HandlePhasing();
    }

    void FixedUpdate()
    {
        currentState.PhysicsUpdate();
        if (!isCameraBound)
        {
            cameraBounds.transform.position = playerObject.transform.position;
        }
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


    void OnUnBindCamera(InputValue UnBindCamera) // for debuggin mostly
    {
        CenterCamera();
        isCameraBound = !isCameraBound;
        Debug.Log(isCameraBound);
    }

    void CenterCamera()
    {
            cameraBounds.transform.position = cameraCenter;
    }

}
