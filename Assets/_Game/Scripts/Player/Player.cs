using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public PlayerState currentState;

    public Rigidbody2D rb;
    public CircleCollider2D circleCollider;
    
    public Vector2 direction;
    [SerializeField] public float acceleration = 0.1f;
    [SerializeField] public float deceleration = 0.2f;
    [SerializeField] public float maxSpeed = 10f;
    

    void Start()
    {
        ChangeState(new PlayerIdleState(this));
    }

    void Update()
    {
        currentState.Update();
    }

    void FixedUpdate()
    {
        currentState.PhysicsUpdate();
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

    void OnMove(InputValue input)
    {
        direction = input.Get<Vector2>();
    }
}
