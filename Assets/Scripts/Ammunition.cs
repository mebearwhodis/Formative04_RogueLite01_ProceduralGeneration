using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammunition : MonoBehaviour
{
    private Rigidbody2D _rb;
    [SerializeField] private float _ammoSpeed = 10f;

     
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject,5);
        
        _rb.rotation = transform.rotation.eulerAngles.z;
    }
    
    private void Update()
    {
        _rb.velocity =  transform.right * _ammoSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      
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