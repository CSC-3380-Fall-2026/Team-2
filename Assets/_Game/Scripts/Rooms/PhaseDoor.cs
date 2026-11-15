using UnityEngine;
using UnityEngine.Tilemaps;

//Phaseable door that locks behind the player once they phase INTO the room.
public class PhaseDoor : MonoBehaviour
{
    [SerializeField] private Tilemap doorTilemap;
    [SerializeField] private Color lockedTint = new Color(0.55f, 0.25f, 0.25f, 1f);
    //The direction the player has to enter through the door to lock it
    [SerializeField] private Vector2 inwardDirection; 
    [SerializeField] private int lockedLayer = 0;

    private Collider2D zoneCollider;
    private bool playerInZone;
    private Vector2 entryPosition;
    private int unlockedLayer;
    private Color unlockedColor;

    public bool IsLocked { get; private set; }
    public static bool ShouldLock(Vector2 entryPosition, Vector2 exitPosition, Vector2 doorCenter, Vector2 inwardDirection)
    {
        bool enteredFromOutside = Vector2.Dot(entryPosition - doorCenter, inwardDirection) < 0f;
        bool exitedInside = Vector2.Dot(exitPosition - doorCenter, inwardDirection) > 0f;
        return enteredFromOutside && exitedInside;
    }

    void Awake()
    {
        zoneCollider = GetComponent<Collider2D>();
        Player.Respawned += ResetDoor;
    }

    void OnDestroy()
    {
        Player.Respawned -= ResetDoor;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsLocked || other.GetComponentInParent<Player>() == null)
        {
            return;
        }

        playerInZone = true;
        entryPosition = other.transform.position;
    }

    //Only locks on exit to ensure the player is never inside of the wall when it locks
    void OnTriggerExit2D(Collider2D other)
    {
        if (IsLocked || !playerInZone || other.GetComponentInParent<Player>() == null) 
            return;
        playerInZone = false;
        if (ShouldLock(entryPosition, other.transform.position, zoneCollider.bounds.center, inwardDirection))
        {
            Lock();
        }
    }

    public void Lock()
    {
        IsLocked = true;
        unlockedLayer = doorTilemap.gameObject.layer;
        unlockedColor = doorTilemap.color;
        doorTilemap.gameObject.layer = lockedLayer;
        doorTilemap.color = lockedTint;
    }

    //Resets the doors when the player dies so that they're phasable again
    public void ResetDoor()
    {
        playerInZone = false;
        if (!IsLocked)
        {
            return;
        }

        IsLocked = false;
        doorTilemap.gameObject.layer = unlockedLayer;
        doorTilemap.color = unlockedColor;
    }
}
