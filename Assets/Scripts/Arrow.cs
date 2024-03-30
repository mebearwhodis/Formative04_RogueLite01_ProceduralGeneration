using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D _rb;
    private BoxCollider2D _bc;
    [SerializeField] private float _arrowSpeed = 1f;

     
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _bc = GetComponent<BoxCollider2D>();
        Destroy(gameObject,5);
    }
    
    private void Update()
    {
        //transform.rotation = Quaternion.LookRotation(_rb.velocity);
        _rb.velocity = Vector2.down * _arrowSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      
        Debug.Log("ArrowCollided");
        //transform.position = collision.contacts[0].point;
        //transform.SetParent(collision.transform);
         
        //_rb.isKinematic = true;
        //_rb.velocity = Vector3.zero;
        //_rb.angularVelocity = Vector3.zero;
         
        if (other.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Hit wall"); 
            Destroy(gameObject);
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Pewpew");
            Destroy(other.gameObject);
        }

       
    }
}