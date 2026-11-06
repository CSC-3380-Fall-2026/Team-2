using System;
using UnityEngine;

public class Germ1EnemyChase : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float stopDistance = 1f;

    private Rigidbody2D rb;
    private Animator animator;
    public float rollDuration = 1f;
    public float idleDuration = 2f;
    private bool isRolling = true;
    private float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        timer = rollDuration;
    }

    // Update is called once per frame
    void Update()
    {
        timer = timer - Time.fixedDeltaTime;
        bool IsMoving = false;

        if (timer <= 0f)
        {
            isRolling = !isRolling;
            timer = isRolling ? rollDuration : idleDuration;

        }

      


        if (isRolling && player != null)
        {

            Vector2 direction = (Vector2)player.position - rb.position;
            float distance = direction.magnitude;



            if (distance > stopDistance && speed > 0f)
            {
                IsMoving = true;

                float step = Mathf.Min(speed * Time.fixedDeltaTime, distance - stopDistance);

                rb.MovePosition(rb.position + direction.normalized * speed * step);
            }
        }
        animator.SetBool("IsMoving", IsMoving);
    }
}
