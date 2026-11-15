using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FallingFloor : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;        // drag FallingFloor's Tilemap here
    [SerializeField] private Transform player;       // drag the Player here
    [SerializeField] private float fallDelay = 1.5f;   // seconds before the tile disappears

    // The "guest list": cells that have already started falling
    private HashSet<Vector3Int> triggeredCells = new HashSet<Vector3Int>();

    void Update()
    {
        Vector3Int cell = tilemap.WorldToCell(player.position);

        // TODO: if there's no tile at this cell, stop here
        if (tilemap.GetTile(cell) == null)
        {
            return;
        }
        // TODO: if this cell is already on the list, stop here
        if (triggeredCells.Contains(cell))
        {
            return;
        }
        // TODO: add the cell and the 8 cells around it to the list and start its fall
        triggeredCells.Add(cell);
        StartCoroutine(FallRoutine(cell));
    }

    private IEnumerator FallRoutine(Vector3Int cell)
    {
        
        // TODO: shake the tile (we'll do this part last)

        // TODO: wait fallDelay seconds
        yield return new WaitForSeconds(fallDelay);
        // TODO: remove the tile from the tilemap
        tilemap.SetTile(cell, null);
        tilemap.SetTile(cell + Vector3Int.up, null);
        tilemap.SetTile(cell + Vector3Int.down, null);
        tilemap.SetTile(cell + Vector3Int.left, null);
        tilemap.SetTile(cell + Vector3Int.right, null);
    }
}