using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public PlayerState currentState;
    public Animator animator;
    private Coroutine speedResetCoroutine;

    [Header("Animations")]
    public AnimationClip bounce;
    public AnimationClip TakeDamage;

    public string bounceName = "Bounce";
    public string damageName = "TakeDamage";

    public float hurtCoolDown = 1f;

    private float bounceDur;
    private float invisFrame;

    private bool playBounce;
    private bool playTakeDamage;

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

        UpdateRollAnimation();

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

    //
    void UpdateRollAnimation()
    {
        if (playBounce)
        {
            //let bounce play before roll starts again 
            if (Time.time < bounceDur)
                return;

            playBounce = false;
            playTakeDamage = false;
            animator.Play("Ratroll", 0, 0f);

        }
        Vector2 velocity = rb.linearVelocity;
        float speed = velocity.magnitude;

        //playback sceen to actaul movement speed              
        animator.speed = Mathf.Clamp(rb.linearVelocity.magnitude / 10f, 0f, 3f);

        if (speed < 0.05f)
            return;

        Vector2 travelDirection = velocity.normalized;

        bool diagonal = Mathf.Abs(travelDirection.x) > 0.38f && Mathf.Abs(travelDirection.y) > 0.38f;

        string animationName = diagonal ? "RatRollDiagonal" : "Ratroll";

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            animator.Play(animationName, 0, 0f);
        }

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
    void OnCollisionEnter2D(Collision2D collision)
    {
        //priority for enemy collision over wall
        if (isEnemy(collision.collider))
        {
            TryPlayTakeDamage();
            return;
        }
        if (collision.collider.sharedMaterial != bounceMaterial)
        {
            return;
        }

        maxSpeed = Mathf.Min(collision.relativeVelocity.magnitude * bounceSpeedMultiplier, maxBounceSpeed);

        if (speedResetCoroutine != null)
        {
            StopCoroutine(speedResetCoroutine);
        }

        speedResetCoroutine = StartCoroutine(ResetMaxSpeed());

        if (!playTakeDamage)
        {
            playResponse(bounceName, bounce , false);
         }
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

    void checkRoom()
    {
        if (!isCameraBound)
        {
            cameraBounds.transform.position = playerObject.transform.position;
        }
        if (gameObject.transform.position.x <= cameraBounds.transform.position.x - 18)
        {
            EnterRoom(Vector2.left);
            //OnUnBindCamera();
        }
        if (gameObject.transform.position.x >= cameraBounds.transform.position.x + 18)
        {
            EnterRoom(Vector2.right);
            //OnUnBindCamera();
        }
        if (gameObject.transform.position.y <= cameraBounds.transform.position.y - 10)
        {
            EnterRoom(Vector2.down);
            //OnUnBindCamera();
        }
        if (gameObject.transform.position.y >= cameraBounds.transform.position.y + 10)
        {
            EnterRoom(Vector2.up);
            //OnUnBindCamera();
        }
    }

    void EnterRoom(Vector2 roomDirection)
    {
        //temporary solution, later we will get the direct point to the enemies from save file
        GameObject parentObj = GameObject.Find("leftRoom"); ;
        if (roomDirection == Vector2.left)
        {
            //parentObj = GameObject.Find("leftRoom");
            cameraCenter.x = cameraCenter.x - 36;
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
        }
        if (roomDirection == Vector2.down)
        {
            //parentObj = GameObject.Find("downRoom");
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isEnemy(other))
        {
            TryPlayTakeDamage();
        }
    }


    bool isEnemy(Collider2D other)
    {
        //starts collider on child of enemy
        Transform target = other.transform;

        while (target != null)
        {
            if (target.CompareTag("Enemy"))
            {
                return true;
            }

            target = target.parent;

        }
        return false;

    }
    void TryPlayTakeDamage()
    {
        if (Time.time < invisFrame)
        {
        return;
        }

        if (playResponse(damageName,TakeDamage , true))

        {
            invisFrame = Time.time + Mathf.Max(hurtCoolDown, TakeDamage.length);
        }
    }

    bool playResponse(string stateName, AnimationClip clip, bool isHurt)
    {

        if (animator == null || clip == null)
        {
            //console warning 
            Debug.LogWarning("Assingn animator and Animation clips", this);
            return false;
        }
        int stateHash = Animator.StringToHash("Base Layer." + stateName);

        if (!animator.HasState(0, stateHash))
        {
            Debug.LogWarning("Animator sate not " + stateName, this);
                return false;
        }

        playBounce = true;
        playTakeDamage = isHurt;
        bounceDur = Time.time + clip.length;

        animator.speed = 1f;
        animator.Play(stateHash, 0, 0f);

        return true;
    }

}
