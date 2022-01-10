using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float projectileSpeed = 30f, projectileForce = 30f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime * projectileSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Apply a force on the enemy if he is hit by the projectile
        if (other.CompareTag("Enemy"))
        {
            Rigidbody enemyRigidbody = other.GetComponent<Rigidbody>();
            Vector3 awayFromProjectile = (other.transform.position - transform.position).normalized;

            enemyRigidbody.AddForce(awayFromProjectile * projectileForce, ForceMode.Impulse);
        }
    }
}
