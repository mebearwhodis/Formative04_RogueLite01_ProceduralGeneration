using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
  [SerializeField] private float _moveSpeed = 5f;
  private float _speedMultiplier = 1f;
  private Rigidbody2D _rb;
  private Animator _animator;
  private SpriteRenderer _sr;
  private PlayerControls _playerControls;
  private Vector2 _movement;
  private Vector2 _lookDirection;
  
  //Hitboxes of the different attack directions
  [SerializeField] private GameObject _hitBox_Top;
  [SerializeField] private GameObject _hitBox_Bottom;
  [SerializeField] private GameObject _hitBox_Left;
  [SerializeField] private GameObject _hitBox_Right;
  
  private bool _canRoll = true;
  private bool _canShoot = true;
  private bool _canDropBomb = true;
  private bool _canAttack = true;
  private bool _canMove = true;
  private bool _canBlock = true;
  
  private bool _isRolling = false;

  public bool CanRoll
  {
    get => _canRoll;
    set => _canRoll = value;
  }
  public bool IsRolling => _isRolling;

  [Header("Items")] 
  [SerializeField] private Bomb _bomb;
  [SerializeField] private Arrow _arrow;

  private void Awake()
  {
    _playerControls = new PlayerControls();
  }

  private void Start()
  {
    _rb = GetComponent<Rigidbody2D>();
    _animator = GetComponent<Animator>();
    _sr = GetComponent<SpriteRenderer>();
    
    //Subscribe to Button type actions
    _playerControls.Player.Attack.performed += _ => Attack();
    _playerControls.Player.Bomb.performed += _ => DropBomb();
    _playerControls.Player.Shoot.performed += _ => ShootArrow();
    _playerControls.Player.Roll.performed += _ => Roll();
    _playerControls.Player.Block.performed += _ => StartBlock();
    _playerControls.Player.Block.canceled += _ => StopBlock();
  }

  private void OnEnable()
  {
    _playerControls.Enable();
  }

  private void OnDisable()
  {
    if(_playerControls != null)
      _playerControls.Disable();
  }

  private void Update()
  {
    PlayerInput();
    
    //Bif bof moyen de faire
    //_animator.speed = (IsNotMoving() && !_isRolling) ? 0 : 1;
  }
  
  private void FixedUpdate()
  {
    Move();
  }

  private void PlayerInput()
  {
    if(!_canMove){return;}
    
    _movement = _playerControls.Player.Move.ReadValue<Vector2>();
    _lookDirection = _playerControls.Player.Look.ReadValue<Vector2>();
    
    if(IsNotLooking())
    {
      _lookDirection = _movement;
    }
    
    _animator.SetFloat("lookX", _lookDirection.x);
    _animator.SetFloat("lookY", _lookDirection.y);

    if (_lookDirection.x >= 0.5f || _lookDirection.x <= -0.5f || _lookDirection.y >= 0.5f || _lookDirection.y <= -0.5f)
    {
      _animator.SetFloat("lastLookX", _lookDirection.x);
      _animator.SetFloat("lastLookY", _lookDirection.y);
    }
  }

  private bool IsNotMoving()
  {
    return (_movement.x is < 0.1f and > -0.1f && _movement.y is < 0.1f and > -0.1f);
  }

  private bool IsNotLooking()
  {
    return (_lookDirection.x is < 0.1f and > -0.1f && _lookDirection.y is < 0.1f and > -0.1f);
  }
  
  private void Move()
  {
    if(!_canMove){return;}
    if (IsNotMoving())
    {
      _animator.SetBool("isMoving", false);
      return;
    }
    _animator.SetBool("isMoving", true);
    _rb.MovePosition(_rb.position + _movement * (_moveSpeed * _speedMultiplier * Time.fixedDeltaTime));
  }

  private void Attack()
  {
    _rb.velocity = Vector2.zero;
    _canMove = false;
    _animator.SetTrigger("attack");
  }
  
  private void ActivateCollider()
  {
    if (_animator.GetFloat("lastLookX") <= -0.5f)
    {
      _hitBox_Left.SetActive(true);
    }
    if (_animator.GetFloat("lastLookX") >= 0.5f)
    {
      _hitBox_Right.SetActive(true);
    }
    if (_animator.GetFloat("lastLookY") <= -0.5f)
    {
      _hitBox_Bottom.SetActive(true);
    }
    if (_animator.GetFloat("lastLookY") >= 0.5f)
    {
      _hitBox_Top.SetActive(true);
    }
  }

  private void CanMoveFunction()
  {
    _canMove = true;
    
    _hitBox_Left.SetActive(false);
    _hitBox_Right.SetActive(false);
    _hitBox_Bottom.SetActive(false);
    _hitBox_Top.SetActive(false);
  }

  private void DropBomb()
  {
    _rb.velocity = Vector2.zero;
    _canMove = false;
    
    _animator.SetTrigger("useItem");
    
    Instantiate(_bomb, transform.position, Quaternion.identity);
  }

  private void ShootArrow()
  {  
    _rb.velocity = Vector2.zero;
    _canMove = false;
    
    _animator.SetTrigger("useItem");
    
    float angle = 0;
    
    if (IsNotLooking())
    {
      angle = Mathf.Atan2(_animator.GetFloat("lastLookY"), _animator.GetFloat("lastLookX")) * Mathf.Rad2Deg;
    }
    else
    {
      // Calculate the angle of the look direction
      angle = Mathf.Atan2(_lookDirection.y, _lookDirection.x) * Mathf.Rad2Deg;
    }
    
    Instantiate(_arrow, transform.position, Quaternion.AngleAxis(angle, Vector3.forward));
  }

  private void Roll()
  {
    if(_canRoll == false || _isRolling || IsNotMoving()){return;}

    Debug.Log("Do a barrel roll!");
    _animator.SetTrigger("roll");
    if (_movement.x > 0)
    {
      _sr.flipX = true;
    }
    _isRolling = true;
    _canRoll = false;
      
    StartCoroutine("RollCooldown");
  }
  
  IEnumerator RollCooldown()
  {
    yield return new WaitForSeconds(0.6f);
    _sr.flipX = false;
    
    yield return new WaitForSeconds(0.4f);
    _isRolling = false;
    _canRoll = true;
  }

  private void StartBlock()
  {
    _canRoll = false;
    _speedMultiplier = 0.5f;
    _animator.SetBool("isBlocking", true);
    
    //Also if not looking, take the last move or look direction
  }

  private void StopBlock()
  {
    _canRoll = true;
    _speedMultiplier = 1f;
    _animator.SetBool("isBlocking", false);
  }
}
