using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D _rb;
    private BoxCollider2D _bc;
    [SerializeField] private float _arrowSpeed = 1f;


    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _bc = GetComponent<BoxCollider2D>();
        Destroy(gameObject, 5);

        _rb.rotation = transform.rotation.eulerAngles.z;
    }

    private void Update()
    {
        _rb.velocity = transform.right * _arrowSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }
}