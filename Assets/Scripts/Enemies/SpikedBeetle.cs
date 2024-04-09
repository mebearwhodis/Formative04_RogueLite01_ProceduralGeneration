using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikedBeetle : MonoBehaviour
{
    public float wanderForce = 10f;
    public float chargeForce = 100f;
    public float detectionRange = 5f;

    private Rigidbody2D rb;
    private Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Start wandering
        InvokeRepeating("Wander", 0f, 2f);
    }

    void Update()
    {
        // Check if player is within detection range
        if (Mathf.Abs(player.position.x - transform.position.x) < detectionRange ||
            Mathf.Abs(player.position.y - transform.position.y) < detectionRange)
        {
            Charge();
        }
    }

    void Wander()
    {
        // Apply random force for wandering
        Vector2 randomForce = Random.insideUnitCircle.normalized * wanderForce;
        rb.AddForce(randomForce, ForceMode2D.Impulse);
    }

    void Charge()
    {
        // Calculate direction towards player
        Vector2 direction = (player.position - transform.position).normalized;

        // Apply force to charge towards player
        rb.AddForce(direction * chargeForce, ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Stop charging if colliding with a wall or obstacle
        if (collision.gameObject.CompareTag("Wall"))
        {
            rb.velocity = Vector2.zero;
        }
    }
}