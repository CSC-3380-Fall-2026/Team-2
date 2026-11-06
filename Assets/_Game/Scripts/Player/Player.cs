using System.Collections;
using System.Data.Common;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    
    public bool isSpinning;
    public bool isLaunching;


    [Header("Components")]
    public Rigidbody2D rb;
    public CircleCollider2D playerCollider;
    public GameObject playerObject;
    [SerializeField] private static Player instance;
    
    [SerializeField] public CameraManager cameraManager;
    
    public GameObject cameraBounds;
    public Vector2 cameraCenter;

    [Header("Movement Settings")]
    [SerializeField] public float acceleration = 0.1f;
    [SerializeField] public float deceleration = 0.2f;
    [SerializeField] public float maxSpeed = 20f;

    [SerializeField] public float baseMaxSpeed = 20f;

    private float skillMaxSpeedBonus = 0f;
    private float bounceMaxSpeedBonus = 0f;

    public float maxSpeed => baseMaxSpeed + skillMaxSpeedBonus + bounceMaxSpeedBonus;
    public Vector2 direction;
    //Perk Stuff
    [Header("Shield Settings")]
    [SerializeField] public float numOfShield = 0;
    //[SerializeField] public float maxSpeed = 20f;

    [Header("Death Fields")]
    [SerializeField] private deathHandler playerDeathHandler;
    [SerializeField] private LayerMask deathLayerMask;
    [SerializeField] private LayerMask floorLayerMask;
    public bool isDead{get; private set;}
    private bool physicsSimmed;

    [Header("Skill Points")]
    [SerializeField] public int skillPoints = 5;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(playerObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(playerObject);
        gameObject.tag = "Player";
    }
    void Start()
    {

        ChangeState(new PlayerIdleState(this));
        //getCameraBounds();
    }

    void Update()
    {
        
        //If player is not touching the floor, the player should die
        if (playerCollider.IsTouchingLayers(deathLayerMask) && !playerCollider.IsTouchingLayers(floorLayerMask)) {
            Death();
        }

        currentState.Update();

        UpdateRollAnimation();

        HandlePhasing();
    }

    void FixedUpdate()
    {
        if (isDead) {
            return;
        }

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
        if (isDead) {
            return;
        }

        direction = move.Get<Vector2>();
    }

    void OnSpin(InputValue spin)
    {
        if (isDead) {
            return;
        }
        if (spin.isPressed && Keyboard.current != null && Keyboard.current.ctrlKey.isPressed) {
            return;
        }

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
        if (isDead) {
            return;
        }

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
        float bounceSpeed = Mathf.Min(
            bounce.relativeVelocity.magnitude * bounceSpeedMultiplier,
            maxBounceSpeed
        );

        bounceMaxSpeedBonus = Mathf.Max(0f, bounceSpeed - (baseMaxSpeed + skillMaxSpeedBonus));

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

        bounceMaxSpeedBonus = 0f;
        speedResetCoroutine = null;
    }


    void OnUnBindCamera() // for debuggin mostly
    {
        if (cameraManager != null)
        {
            cameraManager.ToggleCameraBound();
        }
    }

    void checkRoom()
    {
        if (gameObject.transform.position.x <= cameraBounds.transform.position.x - 18)
        {
            cameraManager.EnterRoom(Vector2.left);
            //OnUnBindCamera();
        }
        if (gameObject.transform.position.x >= cameraBounds.transform.position.x + 18)
        {
            cameraManager.EnterRoom(Vector2.right);
            //OnUnBindCamera();
        }
        if (gameObject.transform.position.y <= cameraBounds.transform.position.y - 10)
        {
            cameraManager.EnterRoom(Vector2.down);
            //OnUnBindCamera();
        }
        if (gameObject.transform.position.y >= cameraBounds.transform.position.y + 10)
        {
            cameraManager.EnterRoom(Vector2.up);
            //OnUnBindCamera();
        }
    }

    public void ApplySkills(Skill skill)
    {
        numOfShield += skill.shieldEffect;
        acceleration += skill.accelerationEffect;
        deceleration += skill.deccelerationEffect;
        skillMaxSpeedBonus += skill.maxSpeedEffect;
    }
    //Checks if player is dead, has a handler, and has a rigidbody. If none are true, player dies,
    //freezes, and all controls are taken away, until the total respawn time (3 secs) has elapsed.
    public void Death() 
    {
        
        
        if (isDead) { 
            return;
        }
        if (playerDeathHandler == null || !playerDeathHandler.isActiveAndEnabled) {
            Debug.LogError("Add active deathHandler to the Player prefab. Player.cs", this);
            return;
        }
        if (rb == null) {
             Debug.LogError("Player missing RigidBody. Player.cs", this);
             return;
        }
        
        isDead = true;
        Debug.Log("Debug Kill Called.");
        direction = Vector2.zero;
        currentCharge = 0f;
        ChangeState(new PlayerIdleState(this));
        isSpinning = false;
        isLaunching = false;
        physicsSimmed = rb.simulated;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.simulated = false;

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
        //Death Amim Starts Here V

        if (!playerDeathHandler.handleDeath(finishDeath)) 
        {
            finishDeath();
        }
    }

    //Restores control to player.
    private void finishDeath() 
    {
        //Death Anim Finish Here V

        direction = Vector2.zero;
        currentCharge = 0f;
        isSpinning = false;
        isLaunching = false;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.simulated = physicsSimmed;
        ChangeState(new PlayerIdleState(this));
        isDead = false;
    }

}
