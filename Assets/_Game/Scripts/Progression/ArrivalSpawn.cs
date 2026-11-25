using UnityEngine;

//Moves a player who arrives inside this area onto a safe spot, so a room transition can't drop them on a hazard.
//Saving is not done here; checkpoints are saved through checkpointHandler (press O near a checkpointIdent).
[RequireComponent(typeof(Collider2D))]
public class ArrivalSpawn : MonoBehaviour
{
    [SerializeField] private Transform spawn;

    //Runs before the scene's first physics step, so the player never touches whatever was at the arrival point.
    void Start()
    {
        Player player = Player.instance;
        if (player == null || spawn == null)
        {
            return;
        }

        if (GetComponent<Collider2D>().OverlapPoint(player.transform.position))
        {
            player.restoreCheckpointPos(spawn.position);
        }
    }
}
