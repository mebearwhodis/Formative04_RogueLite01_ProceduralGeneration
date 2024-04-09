using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShyGuy : MonoBehaviour
{
    [SerializeField] private float _maxSpeed;

    [Header("Seek")] 
    [SerializeField] private Transform _seekTarget;
    [SerializeField] private float _seekWeight = 1f;


    private Rigidbody2D _rb;
    private Vector2 _acceleration;
    private Vector2 _seekDesiredVelocity;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _seekTarget = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        Vector2 velocity = _rb.velocity;

        _acceleration = Vector2.zero;
        _acceleration += _seekWeight * Seek(_seekTarget);

        velocity += _acceleration * Time.deltaTime;

        if (velocity.magnitude > _maxSpeed)
        {
            velocity = velocity.normalized * _maxSpeed;
        }

        _rb.velocity = velocity;
    }

    private Vector2 Seek(Transform target)
    {
        Vector2 steeringForce = new Vector2();

        _seekDesiredVelocity = target.position - transform.position;
        steeringForce = _seekDesiredVelocity - _rb.velocity;


        return steeringForce;
    }
}
