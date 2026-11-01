using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public bool spawnWhenEnabled = true;
    public bool spawnOnce = true;

    private bool alreadySpawned;
    private List<GameObject> enemies = new List<GameObject>();

    void OnEnable()
    {
        if (spawnWhenEnabled) SpawnEnemies();
    }

    public void SpawnEnemies()
    {
        if (enemyPrefab == null || (spawnOnce && alreadySpawned)) return;

        if (spawnPoints == null || spawnPoints.Length == 0)
            enemies.Add(Instantiate(enemyPrefab, transform.position, Quaternion.identity));
        else
        {
            foreach (Transform point in spawnPoints)
            {
                if (point != null)
                    enemies.Add(Instantiate(enemyPrefab, point.position, point.rotation));
            }
        }
        alreadySpawned = true;
    }

    public void ClearEnemies()
    {
        foreach (GameObject enemy in enemies)
            if (enemy != null) Destroy(enemy);
        enemies.Clear();
        alreadySpawned = false;
    }
}
