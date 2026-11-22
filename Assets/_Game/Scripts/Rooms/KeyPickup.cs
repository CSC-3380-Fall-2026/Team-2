using UnityEngine;

//Key the player picks up by touching it; it then trails behind the player
//Goes back to where it started when the player respawns after dying
public class KeyPickup : MonoBehaviour
{
    //Makes the key trail slightly behind the player
    [SerializeField] private Vector2 followOffset = new Vector2(-0.8f, 0.8f);
    [SerializeField] private float followSmoothTime = 0.15f;

    private Transform player;
    private Vector3 followVelocity;
    private Vector3 startPosition;

    public bool IsCollected { get; private set; }

    void Awake()
    {
        Player.Respawned += ResetKey;
    }

    void OnDestroy()
    {
        Player.Respawned -= ResetKey;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Player touchedPlayer = other.GetComponentInParent<Player>();
        if (touchedPlayer == null)
        {
            return;
        }

        IsCollected = true;
        startPosition = transform.position;
        player = touchedPlayer.transform;
        GetComponent<Collider2D>().enabled = false;
    }

    void Update()
    {
        if (!IsCollected || player == null)
        {
            return;
        }

        Vector3 target = player.position + (Vector3)followOffset;
        target.z = transform.position.z;
        transform.position = Vector3.SmoothDamp(transform.position, target, ref followVelocity, followSmoothTime);
    }

    //Called when the key is used on the exit
    public void Consume()
    {
        gameObject.SetActive(false);
    }

    //Resets the Key whenever the player respawns
    public void ResetKey()
    {
        if (IsCollected)
        {
            transform.position = startPosition;
        }

        IsCollected = false;
        player = null;
        followVelocity = Vector3.zero;
        GetComponent<Collider2D>().enabled = true;
        gameObject.SetActive(true);
    }
}
