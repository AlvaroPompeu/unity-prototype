using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody enemyRigidbody;
    private Transform playerTransform;
    private SpawnManager spawnManager;
    public GameObject minionPrefab;
    public bool harderEnemy;
    private int numberOfMinions = 2;

    // Start is called before the first frame update
    void Start()
    {
        // Get the spawn manager
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();

        // Get the enemy rigid body
        enemyRigidbody = GetComponent<Rigidbody>();

        // Get the player transform
        playerTransform = GameObject.Find("Player").transform;

        // Make the boss spawn minions every 3 seconds after an initial delay of 1 second
        InvokeRepeating("SpawnMinions", 1f, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        // If the game is active, the enemy must chase the player
        if (!PlayerController.gameOver)
        {
            ChasePlayer();

            // Destroy the enemy when he falls and decrement the enemy count
            if (transform.position.y < -10)
            {
                spawnManager.enemyCount--;
                Destroy(gameObject);
            }
        }
        // Make the enemy stop moving when the game ends
        else
        {
            enemyRigidbody.velocity = Vector3.zero;
        }
    }

    private void SpawnMinions()
    {
        // Spawn the minions close to the boss
        for (int i = 0; i < numberOfMinions; i++)
        {
            Instantiate(minionPrefab, GenerateMinionPosition(), minionPrefab.transform.rotation);
        }

        // Increase the enemy count
        spawnManager.enemyCount += numberOfMinions;
    }

    private Vector3 GenerateMinionPosition()
    {
        // Create a array of possible values for the minion initial offset relative to the boss
        Vector3[] minionOffset = { new Vector3(2f, 0, 0), new Vector3(-2f, 0, 0), new Vector3(0, 0, 2f), new Vector3(0, 0, -2f) };
        
        // Get a random offset from the array
        int randomIndex = Random.Range(0, minionOffset.Length);
        Vector3 minionPosition = transform.position + minionOffset[randomIndex];

        return minionPosition;
    }

    void ChasePlayer()
    {
        // Move the enemy towards the player
        Vector3 chaseDirection = (playerTransform.position - transform.position).normalized;
        enemyRigidbody.AddForce(chaseDirection * moveSpeed * Time.deltaTime, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the boss collides with the player, he must apply a force to him
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromEnemy = (collision.gameObject.transform.position - transform.position).normalized;

            playerRigidbody.AddForce(awayFromEnemy * 20, ForceMode.Impulse);
        }
    }
}
