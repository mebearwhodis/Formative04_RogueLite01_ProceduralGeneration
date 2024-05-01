using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
    [Header("Movement")] 
    [SerializeField] private float _moveSpeed = 5f;
    private float _speedMultiplier = 1f;
    private Vector2 _movement;
    private Vector2 _lookDirection;
    
    //Health
    [Header("Health")] 
    private int _maxPlayerHealth = 6;
    [SerializeField][Range(0,6)] private int _remainingHealth;
    [SerializeField] private Image _heart1;
    [SerializeField] private Image _heart2;
    [SerializeField] private Image _heart3;
    [SerializeField] private Sprite _heartFull;
    [SerializeField] private Sprite _heartHalf;
    [SerializeField] private Sprite _heartEmpty;

    [Header("Attack Hitboxes")]
    //Hitboxes of the different attack directions
    [SerializeField] private GameObject _slashHitBox_Up;
    [SerializeField] private GameObject _slashHitBox_Right;
    [SerializeField] private GameObject _slashHitBox_Down;
    [SerializeField] private GameObject _slashHitBox_Left;

    [Header("Shield Hitboxes")]
    //Hitboxes of the different attack directions
    [SerializeField] private GameObject _shieldHitBox_Up;
    [SerializeField] private GameObject _shieldHitBox_Right;
    [SerializeField] private GameObject _shieldHitBox_Down;
    [SerializeField] private GameObject _shieldHitBox_Left;

    [Header("Cooldowns")] 
    private float _attackCD = 0.3f;
    private float _damagedCD = 1f;
    private float _bombCD = 1f;
    private float _shotCD = 0.2f;

    private bool _gamePaused = false;

    //Enablers - blergh
    private bool _canTakeDamage = true;
    private bool _canRoll = true;
    private bool _canShoot = true;
    private bool _canDropBomb = true;
    private bool _canAttack = true;
    private bool _canMove = true;
    private bool _canLook = true;
    private bool _canBlock = true;
    private bool _isRolling = false;
    private bool _isBlocking = false;

    public bool CanTakeDamage => _canTakeDamage;

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
        if (!_canMove || _gamePaused) return;

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
        if (!_canMove || _gamePaused)
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
    
    public void LowerHealth()
    {
        if (!_canTakeDamage) { return; }
        _canTakeDamage = false;
        _remainingHealth--;
        UpdateHearts();
        if (_remainingHealth > 0)
        {
            StartCoroutine(DamagedCooldown());
        }
        else
        {
            Death();
        }
    }

    private void Death()
    {
        _animator.SetTrigger("dead");
        //Deactivate inputs rather than use the bools 'cause it's disgustang
        _canTakeDamage = _canRoll = _canShoot = _canDropBomb = _canAttack = _canMove = _canLook = _canBlock = false;
        _rb.bodyType = RigidbodyType2D.Static;
    }
    
    //Called by the death animation
    public void GameOver()
    {
        GameManager.Instance.SetGameState(GameManager.GameState.GameOver);
    }

    private void UpdateHearts()
    {
        if (_heart1 is null || _heart2 is null || _heart3 is null) return;
        // Determine which sprite to use for each heart
        _heart1.sprite = (_remainingHealth >= 2) ? _heartFull : (_remainingHealth == 1) ? _heartHalf : _heartEmpty;
        _heart2.sprite = (_remainingHealth >= 4) ? _heartFull : (_remainingHealth == 3) ? _heartHalf : _heartEmpty;
        _heart3.sprite = (_remainingHealth >= 6) ? _heartFull : (_remainingHealth == 5) ? _heartHalf : _heartEmpty;
    }
    
    public void GetKnockedBack(Transform damageSource, float knockBackPower)
    {
        Vector2 direction = transform.position - damageSource.position;
        _rb.AddForce(direction.normalized * knockBackPower * _rb.mass, ForceMode2D.Impulse);
        StartCoroutine(KnockBackRoutine());
    }
    private IEnumerator KnockBackRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        _rb.velocity = Vector2.zero;
    }
    
    private void Attack()
    {
        if (!_canAttack || _gamePaused)
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
        if (!_canDropBomb || _gamePaused)
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
        if (_canRoll == false || _isRolling || IsNotMoving()  || _gamePaused)
        {
            return;
        }

        Debug.Log("Do a barrel roll!");
        _animator.SetTrigger("roll");
        if (_movement.x > 0)
        {
            _sr.flipX = true;
        }

        _canTakeDamage = false;
        _isRolling = true;
        _canRoll = false;

        StartCoroutine("RollCooldown");
    }

    private void StartBlock()
    {
        if (_gamePaused)
        {
            return;
        }
        _canRoll = false;
        _isBlocking = true;
        _speedMultiplier = 0.5f;
        _animator.SetBool("isBlocking", true);
    }

    private void StopBlock()
    {
        if (_gamePaused)
        {
            return;
        }
        _canRoll = true;
        _isBlocking = false;
        _speedMultiplier = 1f;
        _animator.SetBool("isBlocking", false);
    }

    private void BlockHitBoxes()
    {
        if (!_isBlocking) 
        { 
            _shieldHitBox_Up.SetActive(false);
            _shieldHitBox_Right.SetActive(false);
            _shieldHitBox_Down.SetActive(false);
            _shieldHitBox_Left.SetActive(false);
            return;
        }
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

    private IEnumerator DamagedCooldown()
    {
        yield return new WaitForSeconds(_damagedCD);
        _canTakeDamage = true;
    }
    
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
        _canTakeDamage = true;
    }

    #endregion
}