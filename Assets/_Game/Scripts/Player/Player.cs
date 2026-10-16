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
    
    [Header("Spin Settings")]
    public float currentCharge;
    public float chargePerMash = 4f;
    public float maxCharge = 30f;
    public float spinDecayRate = 2f;
    public float spinDeceleration = 1f;
    public float spinLaunchDelay = 0.5f;
    public bool isCameraBound = true;
    public bool isSpinning;
    public bool isLaunching;
    

    [Header("Components")]
    public Rigidbody2D rb;
    public CircleCollider2D circleCollider;
    public GameObject playerObject;
    public GameObject cameraBounds;
    
    [Header("Movement Settings")]
    [SerializeField] public float acceleration = 0.1f;
    [SerializeField] public float deceleration = 0.2f;
    [SerializeField] public float maxSpeed = 20f;
    
    public Vector2 direction;

    void Start()
    {
        ChangeState(new PlayerIdleState(this));
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

    void OnUnBindCamera(InputValue UnBindCamera) // for debuggin mostly
    {
        isCameraBound = !isCameraBound;
        Debug.Log(isCameraBound);
    }

}
