using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    private Rigidbody playerRigidbody;
    private Transform focalPoint;
    private bool hasForcePowerup = false, hasJumpPowerup = false, hasPowerup = false;
    private float powerupStrength = 40f, powerupDuration = 7f, smashForce = 50f, smashRadius = 12.5f, gravityMultiplier = 10f;
    public GameObject powerupIndicators, forcePowerupIndicator, projectilePowerupIndicator, jumpPowerupIndicator;
    public GameObject projectilePrefab;
    public static bool gameOver = false;
    public GameObject gameOverUI;

    // Start is called before the first frame update
    void Start()
    {
        // Get player rigid body
        playerRigidbody = GetComponent<Rigidbody>();

        // Get the camera focal point
        focalPoint = GameObject.Find("Focal Point").transform;

        // Increase the game gravity
        Physics.gravity = new Vector3(0, -9.81f * gravityMultiplier, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // If the game is active, allow the player to move
        if (!gameOver)
        {
            MovePlayer();
            // Keep the powerup indicator beneath the player
            powerupIndicators.transform.position = transform.position;

            // Let the player do the smash attack if he is with the correct powerup
            if (hasJumpPowerup)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    SmashAttack();
                }
            }
        }
        // If the game is over, wait for the R key press to restart it
        else
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                gameOver = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void MovePlayer()
    {
        float verticalInput = Input.GetAxis("Vertical");

        // Add force to the player rigid body according to the input
        playerRigidbody.AddForce(focalPoint.forward * verticalInput* moveSpeed * Time.deltaTime, ForceMode.Impulse);

        // If the player falls, the game must end
        if (transform.position.y < -10)
        {
            // Enable the game over UI elements
            gameOverUI.SetActive(true);
            gameOver = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check which power up the player got, and apply the corresponding game mechanic
        // There can be only one power up active at a time
        if (other.CompareTag("ForcePowerup") && !hasPowerup)
        {
            PickPowerup(other.gameObject, "Force", forcePowerupIndicator);  
        }
        else if (other.CompareTag("ProjectilePowerup") && !hasPowerup)
        {
            PickPowerup(other.gameObject, "Projectile", projectilePowerupIndicator);
        }
        else if (other.CompareTag("JumpPowerup") && !hasPowerup)
        {
            PickPowerup(other.gameObject, "Jump", jumpPowerupIndicator);
        }
    }

    private void PickPowerup(GameObject powerup, string type, GameObject indicator)
    {
        Destroy(powerup);
        indicator.SetActive(true);
        hasPowerup = true;

        if (type == "Force")
        {
            hasForcePowerup = true;
        }
        else if (type == "Projectile")
        {
            InvokeRepeating("FireProjectiles", 0, 1f);
        }
        else if (type == "Jump")
        {
            hasJumpPowerup = true;
        }

        // Start a coroutine to disable the power up after its duration
        StartCoroutine(PowerupCountdown(type, indicator));
    }

    IEnumerator PowerupCountdown(string type, GameObject indicator)
    {
        yield return new WaitForSeconds(powerupDuration);
        hasPowerup = false;
        indicator.SetActive(false);

        if (type == "Force")
        {
            hasForcePowerup = false;
        }
        else if (type == "Projectile")
        {
            CancelInvoke("FireProjectiles");
        }
        else if (type == "Jump")
        {
            hasJumpPowerup = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Apply a force to the enemy if the player has the correct power up
        if (collision.gameObject.CompareTag("Enemy") && hasForcePowerup)
        {
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayDirection = (collision.gameObject.transform.position - transform.position).normalized;

            enemyRigidbody.AddForce(awayDirection * powerupStrength, ForceMode.Impulse);
        }
        // Send the enemies flying after a smash attack
        else if (collision.gameObject.CompareTag("Ground") && hasJumpPowerup)
        {
            SendEnemiesFlying();
        }
    }

    private void FireProjectiles()
    {
        Instantiate(projectilePrefab, transform.position, Quaternion.Euler(90, 0, 0));
        Instantiate(projectilePrefab, transform.position, Quaternion.Euler(-90, 0, 0));
        Instantiate(projectilePrefab, transform.position, Quaternion.Euler(90, 90, 0));
        Instantiate(projectilePrefab, transform.position, Quaternion.Euler(90, -90, 0));
        Instantiate(projectilePrefab, transform.position, Quaternion.Euler(90, 45, 0));
        Instantiate(projectilePrefab, transform.position, Quaternion.Euler(90, -45, 0));
        Instantiate(projectilePrefab, transform.position, Quaternion.Euler(90, 135, 0));
        Instantiate(projectilePrefab, transform.position, Quaternion.Euler(90, -135, 0));
    }

    private void SmashAttack()
    {
        // Send the player up
        playerRigidbody.AddForce(Vector3.up * smashForce, ForceMode.Impulse);

        // Start a coroutine to send the player down after a brief delay
        StartCoroutine(Smash());
    }

    IEnumerator Smash()
    {
        yield return new WaitForSeconds(0.15f);
        // Stop the player midair and send him down
        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.AddForce(Vector3.down * smashForce, ForceMode.Impulse);
    }

    private void SendEnemiesFlying()
    {
        // Get all the enemies on the screen
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Apply an explosion force on all enemies on the player location
        for (int i = 0; i < enemies.Length; i++)
        {
            Rigidbody enemyRigidbody = enemies[i].GetComponent<Rigidbody>();
            enemyRigidbody.AddExplosionForce(smashForce, transform.position, smashRadius, 0, ForceMode.Impulse);
        }
    }
}
