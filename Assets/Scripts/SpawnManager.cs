using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public GameObject bossPrefab;
    public GameObject[] powerupPrefabs;
    private float spawnBound = 9;
    public int enemyCount = 0, wave = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // If all enemies are destroyed and the game is active, start the next wave
        if (enemyCount == 0 && !PlayerController.gameOver)
        {
            wave++;

            // Start a boss fight after each 5 waves
            if (wave % 5 == 0)
            {
                StartBossWave(wave);
            }
            else
            {
                StartWave(wave);
            }
        }
    }

    void StartWave(int wave)
    {
        int randomIndex;
        enemyCount = wave;

        // Spawn a random power up
        randomIndex = Random.Range(0, powerupPrefabs.Length);
        Spawn(powerupPrefabs[randomIndex]);

        // Spawn regular or harder enemies
        for (int i = 0; i < wave; i++)
        {
            randomIndex = Random.Range(0, enemyPrefabs.Length);
            Spawn(enemyPrefabs[randomIndex]);
        }
    }

    void StartBossWave(int wave)
    {
        int randomIndex;
        enemyCount = wave / 5;

        // Spawn a random power up
        randomIndex = Random.Range(0, powerupPrefabs.Length);
        Spawn(powerupPrefabs[randomIndex]);

        // Spawn the boss
        for (int i = 0; i < enemyCount; i++)
        {
            Spawn(bossPrefab);
        }
    }

    void Spawn(GameObject prefab)
    {
        Vector3 spawnPosition = generateSpawnPoint();
        Instantiate(prefab, spawnPosition, prefab.transform.rotation);
    }

    Vector3 generateSpawnPoint()
    {
        //Generate a random point inside the arena for the spawn location
        float randomX = Random.Range(-spawnBound, spawnBound);
        float randomZ = Random.Range(-spawnBound, spawnBound);

        return new Vector3(randomX, 0, randomZ);
    }
}
