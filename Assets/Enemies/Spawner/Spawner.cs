using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawner Attributes")]
    [SerializeField]
    private string spawns = "ss-ss-ssss-ssss";
    [SerializeField]
    private float startDelay = 1f;
    [SerializeField]
    private float spawnCooldown = 5f;
    

    [Header("Enemy Prefabs")]
    [SerializeField]
    private GameObject swarmerPrefab = null;

    private bool playerIsAlive = true;


    private void Start() {
        StartCoroutine(SpawnLoop());
        GameManager.instance.onPlayerDeath += StopSpawning;
        GetTotalEnemyCount();
    }


    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(startDelay);    // Spawner waits x-seconds before it starts spawning
        foreach (char enemy in spawns)
        {
            // print("char: " + enemy);
            if(!playerIsAlive)
            {
                break;
            }
            if(char.IsDigit(enemy))
            {
                int amount = enemy - '0';
                // print("amount: " + amount);
                Spawn(swarmerPrefab, amount);
            }
            else
            {
                print("waiting " + spawnCooldown + " seconds");
                yield return new WaitForSeconds(spawnCooldown); // Spawner waits x-seconds before spawning more enemies  
            }
        }
        // a loop cooldown could go here; for now loops only once
    }

    private void Spawn(GameObject enemy, int amount)
    {
        print("Spawning " + amount + " skulls");
        for (int i = 0; i < amount; i++)
        {
            Vector2 pos = RandomPointOnCircleEdge(8f);
            //pos = transform.position;
            Instantiate(enemy, pos, Quaternion.identity, transform);
        }
    }

    private Vector3 RandomPointOnCircleEdge(float radius)
    {
        var vector2 = Random.insideUnitCircle.normalized * radius;
        return vector2;
    }

    private void StopSpawning() {
        print("Stopped Spawning");
        playerIsAlive = false;
    }

    private void GetTotalEnemyCount()
    {
        int total = 0;
        foreach (char c in spawns)
        {
            if(char.IsDigit(c))
            {
                int i = c - '0';
                total += i;
            }
        }

        GameManager.instance.SetEnemyTotal(total);
    }
}
