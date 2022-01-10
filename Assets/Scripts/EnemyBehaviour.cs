using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody enemyRigidbody;
    private Transform playerTransform;
    private SpawnManager spawnManager;
    public bool harderEnemy;

    // Start is called before the first frame update
    void Start()
    {
        // Get the spawn manager
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();

        // Get the enemy rigid body
        enemyRigidbody = GetComponent<Rigidbody>();

        // Get the player transform
        playerTransform = GameObject.Find("Player").transform;
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

    void ChasePlayer()
    {
        // Move the enemy towards the player
        Vector3 chaseDirection = (playerTransform.position - transform.position).normalized;
        enemyRigidbody.AddForce(chaseDirection * moveSpeed * Time.deltaTime, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the enemy is the harder one, he must apply a force to the player during collisions
        if (harderEnemy)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
                Vector3 awayFromEnemy = (collision.gameObject.transform.position - transform.position).normalized;

                playerRigidbody.AddForce(awayFromEnemy * 10, ForceMode.Impulse);
            }
        }
    }
}
