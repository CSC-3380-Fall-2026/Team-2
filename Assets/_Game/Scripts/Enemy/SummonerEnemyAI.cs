
using System.Collections.Generic;
using UnityEngine;

public class SummonerEnemyAI : MonoBehaviour
{
    public GameObject minionPrefab;

    public float summonInterval = 5f;

    public float summonRange = 8f;

    public float spawnDistance = 1.5f;

    public int maxMinions = 3;


    private Transform player;

    private float nextSummonTime;

    private List<GameObject> minions = new List<GameObject>();

    private int totalMinionsSpawned = 0; //tracks how many enemies spawn


    void Start()
    {
        nextSummonTime = Time.time + summonInterval;
    }

    void Update()
    {
        if (player == null)
        {
            GameObject target = GameObject.FindGameObjectWithTag("Player");




            if (target != null)


                player = target.transform;
        }



        if (player == null || minionPrefab == null)
            return;




        float distance = Vector2.Distance(
            transform.position, player.position
        );



        if (distance > summonRange)
            return;


        if (Time.time >= nextSummonTime &&

            totalMinionsSpawned < maxMinions)
        {
            SummonMinion();
            totalMinionsSpawned++;
            nextSummonTime = Time.time + summonInterval;
        }

    }

    void SummonMinion()
    {
        Vector2 direction = Random.insideUnitCircle.normalized;



        if (direction == Vector2.zero)


            direction = Vector2.right;

        Vector3 spawnPosition = transform.position +





            (Vector3)(direction * spawnDistance);

        GameObject minion = Instantiate(
            minionPrefab,


            spawnPosition,


            Quaternion.identity
        );

        minions.Add(minion);

        Debug.Log("Summoner spawned a minion!");
    }
}
