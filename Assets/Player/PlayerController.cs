using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  [SerializeField] private float _moveSpeed = 5f;
  private Rigidbody2D _rb;
  private Animator _animator;
  private SpriteRenderer _sr;
  private PlayerInputs _input;

  [Header("Items")] 
  [SerializeField] private Bomb _bomb;
  [SerializeField] private Arrow _arrow;

  private void Start()
  {
    _rb = GetComponent<Rigidbody2D>();
    _animator = GetComponent<Animator>();
    _sr = GetComponent<SpriteRenderer>();
    _input = GetComponent<PlayerInputs>();
  }

  private void FixedUpdate()
  {
    Move();
    UseItems();
  }

  private void Move()
  {
    _rb.velocity = _input.MoveValue * (_moveSpeed * Time.deltaTime);
    // //Left & Right
    // _rb.velocityX = Input.GetAxis("Horizontal") * _moveSpeed * Time.deltaTime;
    //
    // if (Input.GetAxis("Horizontal") * _moveSpeed < -0.001f)
    // {
    //   _sr.flipX = true;
    //   _animator.SetBool("isWalking", true);
    // }
    // else if (Input.GetAxis("Horizontal") * _moveSpeed > 0.001f)
    // {
    //   _animator.SetBool("isWalking", true);
    //   _sr.flipX = false;
    // }
    // else
    // {
    //   _animator.SetBool("isWalking", false);
    // }
    //
    // //Up & Down
    // _rb.velocityY = Input.GetAxis("Vertical") * _moveSpeed * Time.deltaTime;
    //
    // if (Input.GetAxis("Vertical") * _moveSpeed < -0.001f)
    // {
    //   _sr.flipX = true;
    //   _animator.SetBool("isWalking", true);
    // }
    // else if (Input.GetAxis("Vertical") * _moveSpeed > 0.001f)
    // {
    //   _animator.SetBool("isWalking", true);
    //   _sr.flipX = false;
    // }
    // else
    // {
    //   _animator.SetBool("isWalking", false);
    // }
  }

  private void UseItems()
  {
    if (_input.BombDropped)
    {
      Instantiate(_bomb, transform.position, Quaternion.identity);
    }

    if (_input.ArrowShot)
    {
      ShootArrow();
    }
  }

  private void ShootArrow()
  {
    Instantiate(_arrow, transform.position, Quaternion.identity);
  }
}
