using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  //Generic
  private Rigidbody2D _rb;
  private Animator _animator;
  private SpriteRenderer _sr;
  private PlayerControls _playerControls;
  
  //Movement related
  [Header("Movement")]
  [SerializeField] private float _moveSpeed = 5f;
  private float _speedMultiplier = 1f;
  private Vector2 _movement;
  private Vector2 _lookDirection;
  
  [Header("Attacks")]
  //Hitboxes of the different attack directions
  [SerializeField] private GameObject _hitBox_Top;
  [SerializeField] private GameObject _hitBox_Bottom;
  [SerializeField] private GameObject _hitBox_Left;
  [SerializeField] private GameObject _hitBox_Right;

  [Header("Cooldowns")] 
  private float _attackCD = 0.3f;
  private float _bombCD = 1f;
  private float _shotCD = 0.2f;
  
  //Enablers
  private bool _canRoll = true;
  private bool _canShoot = true;
  private bool _canDropBomb = true;
  private bool _canAttack = true;
  private bool _canMove = true;
  private bool _canLook = true;
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

  #region Unity
  
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
  
  #endregion

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
    if (!_canAttack) {return;}
    _rb.velocity = Vector2.zero;
    _canMove = false;
    _canAttack = false;
    _animator.SetTrigger("attack");

    StartCoroutine("AttackCooldown");
  }

  private void DropBomb()
  {
    if (!_canDropBomb) {return;}
    _rb.velocity = Vector2.zero;
    _canMove = false;
    
    _animator.SetTrigger("useItem");
    
    Instantiate(_bomb, transform.position, Quaternion.identity);

    _canDropBomb = false;
    StartCoroutine("BombCooldown");
  }

  private void ShootArrow()
  {
    if (!_canShoot) {return;}
    _rb.velocity = Vector2.zero;
    _canMove = false;
    
    _animator.SetTrigger("useItem");
    
    float angle = 0;
    
    //If there's no aim input, use the last one
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

    _canShoot = false;
    StartCoroutine("ShotCooldown");
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
  
  private bool IsNotMoving()
  {
    return (_movement.x is < 0.1f and > -0.1f && _movement.y is < 0.1f and > -0.1f);
  }

  private bool IsNotLooking()
  {
    return (_lookDirection.x is < 0.1f and > -0.1f && _lookDirection.y is < 0.1f and > -0.1f);
  }

  #region Cooldown Coroutines
  
  private IEnumerator AttackCooldown()
  {
    yield return new WaitForSeconds(_attackCD);
    _canAttack = true;
  }
   
  private IEnumerator BombCooldown()
  {
    yield return new WaitForSeconds(_bombCD);
    _canDropBomb = true;
  }
  
  private IEnumerator ShotCooldown()
  {
    yield return new WaitForSeconds(_shotCD);
    _canShoot = true;
  }

  private IEnumerator RollCooldown()
  {
    yield return new WaitForSeconds(0.6f);
    _sr.flipX = false;
    
    yield return new WaitForSeconds(0.4f);
    _isRolling = false;
    _canRoll = true;
  }

  #endregion
}
