using UnityEngine;

//Solid exit wall that opens when the player bumps it while carrying the key
public class LockedExit : MonoBehaviour
{
    [SerializeField] private KeyPickup key;   // the key that opens the exit

    void Awake()
    {
        Player.Respawned += ResetExit;
    }

    void OnDestroy()
    {
        Player.Respawned -= ResetExit;
    }

    //Relocks the exit when the player dies
    public void ResetExit()
    {
        gameObject.SetActive(true);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (key != null && key.IsCollected && collision.gameObject.CompareTag("Player"))
        {
            key.Consume();
            gameObject.SetActive(false);
        }
    }
}
