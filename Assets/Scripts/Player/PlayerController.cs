using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : Singleton<PlayerController>
{
    //Generic
    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _sr;
    private PlayerControls _playerControls;
    private PlayerInput _playerInput;
    private RoomSpawn _currentRoom;

    public RoomSpawn CurrentRoom
    {
        get => _currentRoom;
        set => _currentRoom = value;
    }

    //Movement related
    [Header("Movement")] [SerializeField] private float _moveSpeed = 5f;
    private float _speedMultiplier = 1f;
    private Vector2 _movement;
    private Vector2 _lookDirection;

    [Header("Attack Hitboxes")]
    //Hitboxes of the different attack directions
    [SerializeField]
    private GameObject _slashHitBox_Up;

    [SerializeField] private GameObject _slashHitBox_Right;
    [SerializeField] private GameObject _slashHitBox_Down;
    [SerializeField] private GameObject _slashHitBox_Left;

    [Header("Shield Hitboxes")]
    //Hitboxes of the different attack directions
    [SerializeField]
    private GameObject _shieldHitBox_Up;

    [SerializeField] private GameObject _shieldHitBox_Right;
    [SerializeField] private GameObject _shieldHitBox_Down;
    [SerializeField] private GameObject _shieldHitBox_Left;

    [Header("Cooldowns")] 
    private float _attackCD = 0.3f;
    private float _bombCD = 1f;
    private float _shotCD = 0.2f;

    private bool _gamePaused = false;

    //Enablers - blergh
    private bool _canRoll = true;
    private bool _canShoot = true;
    private bool _canDropBomb = true;
    private bool _canAttack = true;
    private bool _canMove = true;
    private bool _canLook = true;
    private bool _canBlock = true;
    private bool _isRolling = false;
    private bool _isBlocking = false;

    public bool CanRoll
    {
        get => _canRoll;
        set => _canRoll = value;
    }

    public bool IsRolling => _isRolling;

    [Header("Items")] [SerializeField] private Bomb _bomb;
    [SerializeField] private Ammunition _arrow;

    #region Unity

    protected override void Awake()
    {
        base.Awake();
        _playerControls = new PlayerControls();
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
        _playerInput = GetComponent<PlayerInput>();

        //Subscribe to Button type actions
        _playerControls.Player.Pause.performed += _ => Pause();
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
        if (_playerControls != null)
            _playerControls.Disable();
    }

    private void Update()
    {
        _gamePaused = GameManager.Instance.IsPaused;
        PlayerInput();
        BlockHitBoxes();
    }

    private void FixedUpdate()
    {
        Move();
    }

    #endregion

    private void PlayerInput()
    {
        if (!_canMove) return;

        _movement = _playerControls.Player.Move.ReadValue<Vector2>();

        // Check the control scheme
        var controlScheme = _playerInput.currentControlScheme;

        // Handle input based on the control scheme
        switch (controlScheme)
        {
            case "Gamepad":
                // Use right stick input for aiming
                _lookDirection = _playerControls.Player.Look.ReadValue<Vector2>();
                break;
            case "Keyboard&Mouse":
            {
                // Handle mouse/keyboard input for aiming
                Vector2 mousePosition = _playerControls.Player.MousePosition.ReadValue<Vector2>();
                Vector2 playerPosition = Camera.main.WorldToScreenPoint(transform.position);
                _lookDirection = (mousePosition - playerPosition).normalized;
                break;
            }
        }

        // If there's no input from mouse/keyboard or right stick, use movement input for aiming
        if (_lookDirection.magnitude < 0.1f)
        {
            _lookDirection = _movement;
        }

        _animator.SetFloat("lookX", _lookDirection.x);
        _animator.SetFloat("lookY", _lookDirection.y);

        if (_lookDirection.x >= 0.5f || _lookDirection.x <= -0.5f || _lookDirection.y >= 0.5f ||
            _lookDirection.y <= -0.5f)
        {
            _animator.SetFloat("lastLookX", _lookDirection.x);
            _animator.SetFloat("lastLookY", _lookDirection.y);
        }
    }


    private void Move()
    {
        if (!_canMove)
        {
            return;
        }

        if (IsNotMoving())
        {
            _animator.SetBool("isMoving", false);
            return;
        }

        _animator.SetBool("isMoving", true);
        _rb.MovePosition(_rb.position + _movement * (_moveSpeed * _speedMultiplier * Time.fixedDeltaTime));
    }

    private void Pause()
    {
        GameManager.Instance.SetPause();
    }

    private void Attack()
    {
        if (!_canAttack)
        {
            return;
        }

        _rb.velocity = Vector2.zero;
        _canMove = false;
        _canAttack = false;
        _animator.SetTrigger("attack");

        StartCoroutine("AttackCooldown");
    }

    private void DropBomb()
    {
        if (!_canDropBomb)
        {
            return;
        }

        _rb.velocity = Vector2.zero;
        _canMove = false;

        _animator.SetTrigger("useItem");

        Instantiate(_bomb, transform.position, Quaternion.identity);

        _canDropBomb = false;
        StartCoroutine("BombCooldown");
    }

    private void ShootArrow()
    {
        if (!_canShoot || _gamePaused)
        {
            return;
        }

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
        if (_canRoll == false || _isRolling || IsNotMoving())
        {
            return;
        }

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
        _isBlocking = true;
        _speedMultiplier = 0.5f;
        _animator.SetBool("isBlocking", true);
    }

    private void StopBlock()
    {
        _canRoll = true;
        _isBlocking = false;
        _speedMultiplier = 1f;
        _animator.SetBool("isBlocking", false);
    }

    private void BlockHitBoxes()
    {
        if (!_isBlocking) {return;}

        _shieldHitBox_Up.SetActive(_animator.GetFloat("lastLookY") >= 0.5f);
        _shieldHitBox_Right.SetActive(_animator.GetFloat("lastLookX") >= 0.5f);
        _shieldHitBox_Down.SetActive(_animator.GetFloat("lastLookY") <= -0.5f);
        _shieldHitBox_Left.SetActive(_animator.GetFloat("lastLookX") <= -0.5f);
    }

    private void ActivateCollider()
    {
        if (_animator.GetFloat("lastLookY") >= 0.5f)
        {
            _slashHitBox_Up.SetActive(true);
        }

        if (_animator.GetFloat("lastLookX") >= 0.5f)
        {
            _slashHitBox_Right.SetActive(true);
        }

        if (_animator.GetFloat("lastLookY") <= -0.5f)
        {
            _slashHitBox_Down.SetActive(true);
        }

        if (_animator.GetFloat("lastLookX") <= -0.5f)
        {
            _slashHitBox_Left.SetActive(true);
        }
    }

    private void CanMoveFunction()
    {
        _canMove = true;

        _slashHitBox_Up.SetActive(false);
        _slashHitBox_Right.SetActive(false);
        _slashHitBox_Down.SetActive(false);
        _slashHitBox_Left.SetActive(false);
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