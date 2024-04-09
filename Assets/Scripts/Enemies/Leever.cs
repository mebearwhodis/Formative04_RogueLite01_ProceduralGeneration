using UnityEngine;

public class Leever : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float wanderRadius = 5f;
    public float popOutDuration = 3f;
    public float undergroundDuration = 5f;

    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private Vector2 _startPosition;
    private Vector2 _targetPosition;
    private bool _isUnderground = true;
    private float _lastPopOutTime;
    private float _lastUndergroundTime;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _startPosition = transform.position;
        _lastPopOutTime = Time.time;
        _lastUndergroundTime = Time.time;
    }

    private void Update()
    {
        if (_isUnderground)
        {
            // Check if it's time to pop out
            if (Time.time - _lastUndergroundTime > undergroundDuration)
            {
                // Pop out randomly
                transform.position = _startPosition + Random.insideUnitCircle * wanderRadius;
                _isUnderground = false;
                _sr.enabled = true;
                _lastPopOutTime = Time.time;
            }
        }
        else
        {
            // Move towards the player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Vector2 direction = ((Vector2)player.transform.position - (Vector2)transform.position).normalized;
                _rb.MovePosition(_rb.position + direction * moveSpeed * Time.deltaTime);
            }

            // Check if it's time to go back underground
            if (Time.time - _lastPopOutTime > popOutDuration)
            {
                _isUnderground = true;
                _sr.enabled = false;
                _lastUndergroundTime = Time.time;
            }
        }
    }
}