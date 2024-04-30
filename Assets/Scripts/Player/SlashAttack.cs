using System;
using System.Collections;
using System.Collections.Generic;
using FSM;
using UnityEngine;

public class SlashAttack : MonoBehaviour
{
    [SerializeField] private int _damageValue = 1;
    [SerializeField] private float _knockBackPower = 3;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.GetComponent<FSM_Enemy>().GetKnockedBack(transform, _knockBackPower);
            if(other.GetComponent<FSM_Enemy>().Invulnerable){return;}
            other.GetComponent<FSM_Enemy>().TakeDamage(_damageValue);
        }
    }
}
