using System.Collections;
using System.Collections.Generic;
using FSM;
using UnityEngine;

public class Ammunition : MonoBehaviour
{
    private Rigidbody2D _rb;
    [SerializeField] private float _ammoSpeed = 10f;
    [SerializeField] private bool _playerProjectile = false;
    [SerializeField] private bool _enemyProjectile = false;
    [SerializeField] private int _damageValue = 1;
    [SerializeField] private float _knockBackPower = 3;


    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 5);

        _rb.rotation = transform.rotation.eulerAngles.z;
    }

    private void Update()
    {
        _rb.velocity = transform.right * _ammoSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Hit wall");
            Destroy(gameObject);
        }

        if (_playerProjectile)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                other.GetComponent<FSM_Enemy>().GetKnockedBack(transform, _knockBackPower);
                if (other.GetComponent<FSM_Enemy>().Invulnerable)
                {
                    return;
                }
                else
                {
                    other.GetComponent<FSM_Enemy>().TakeDamage(_damageValue);
                }
            }
        }

        if (_enemyProjectile)
        {
            if (other.gameObject.CompareTag("PlayerBody"))
            {
                other.GetComponentInParent<PlayerController>().GetKnockedBack(transform, _knockBackPower);
                other.GetComponentInParent<PlayerController>().UpdateHealth(-1);
            }

            if (other.gameObject.CompareTag("Shield"))
            {
                Destroy(gameObject);
            }
        }
    }
}