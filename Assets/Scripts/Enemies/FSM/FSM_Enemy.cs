using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace FSM
{
    public class FSM_Enemy : MonoBehaviour
    {
        private Vector3 _spawnPoint;

        public Vector3 SpawnPoint => _spawnPoint;

        private Animator _animator;
        private Rigidbody2D _rigidbody;
        private SpriteRenderer _spriteRenderer;
        private PlayerController _target = PlayerController.Instance;

        public Animator Animator => _animator;
        public Rigidbody2D Rigidbody => _rigidbody;
        public SpriteRenderer SpriteRenderer => _spriteRenderer;
        public PlayerController Target => _target;

        [Header("Enemy stats")] 
        [SerializeField] private int _startingHealth;
        [SerializeField] private int _currentHealth;
        private bool _invulnerable;
        private float _colorCD = 0.1f;
        private Color _baseColor;
        [SerializeField] private bool _contactDamage = false;
        [SerializeField] private int _damageValue = 1;
        [SerializeField] private float _knockBackPower = 3;

        public bool Invulnerable
        {
            get => _invulnerable;
            set => _invulnerable = value;
        }

        public bool ContactDamage
        {
            get => _contactDamage;
            set => _contactDamage = value;
        }

        [Header("Loot")] 
        [SerializeField] private GameObject _heartPickup;
        [SerializeField] private GameObject _coinPickup;

        [Header("Chase State")] 
        [SerializeField] private float _chaseSpeed;
        [SerializeField] private float _targetDistance;
        
        public float ChaseSpeed => _chaseSpeed;
        public float TargetDistance => _targetDistance;

        [Header("Explosion State")] 
        [SerializeField] private float _explosionTimer;
        [SerializeField] private float _explosionRadius;
        [SerializeField] private float _explosionChaseSpeed;
        
        public float ExplosionTimer => _explosionTimer;
        public float ExplosionRadius => _explosionRadius;
        public float ExplosionChaseSpeed => _explosionChaseSpeed;

        [Header("Hide State")] 
        [SerializeField] private float popoutRadius;

        public float PopoutRadius => popoutRadius;

        [Header("Ranged Attack State")] 
        [SerializeField] private float _minAttackDistance;
        [SerializeField] private float _maxAttackDistance;
        [SerializeField] private Ammunition _ammunition;
        [SerializeField] private float _attackCooldown = 1.5f;
        private bool _canShoot = true;

        public float MinAttackDistance => _minAttackDistance;
        public float MaxAttackDistance => _maxAttackDistance;
        public Ammunition Ammunition => _ammunition;

        [Header("Rush State")] 
        [SerializeField] private float _rushSpeed;
        
        public float RushSpeed => _rushSpeed;
        
        [Header("Wander State")] 
        [SerializeField] private float _wanderRadius;
        [SerializeField] private float _wanderSpeed;
        [SerializeField] private float _maxWanderDistance;

        public float WanderRadius => _wanderRadius;
        public float WanderSpeed => _wanderSpeed;
        public float MaxWanderDistance => _maxWanderDistance;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spawnPoint = transform.position;
            _currentHealth = _startingHealth;
            _baseColor = _spriteRenderer.color;
        }

        private void Update()
        {
            DetectDeath();
        }

        public void ThrowProjectile()
        {
            if(!_canShoot){return;}
            StartCoroutine("RangedAttack");
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            _spriteRenderer.color = Color.grey;
            StartCoroutine(SetDefaultColor(_colorCD));
        }

        private void DetectDeath()
        {
            if (_currentHealth <= 0)
            {
                float randomValue = Random.value;
                if (randomValue < 0.25f)
                {
                    Instantiate(_coinPickup, transform.position, Quaternion.identity);
                    Debug.Log("Coin dropped");
                }
                else if (randomValue < 0.34f)
                {
                    Instantiate(_heartPickup, transform.position, Quaternion.identity);
                    Debug.Log("Heart Dropped");
                }
                PlayerController.Instance.CurrentRoom.monstersLeft--;
                
                Destroy(gameObject);
            }
        }

        public void GetKnockedBack(Transform damageSource, float knockBackPower)
        {
            Vector2 direction = transform.position - damageSource.position;
            _rigidbody.AddForce(direction.normalized * knockBackPower * _rigidbody.mass, ForceMode2D.Impulse);
            StartCoroutine(KnockBackRoutine());
        }

        private IEnumerator RangedAttack()
        {
            _canShoot = false;
            Vector2 direction = _target.transform.position - this.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Instantiate(_ammunition, transform.position, Quaternion.AngleAxis(angle, Vector3.forward));
            yield return new WaitForSeconds(_attackCooldown);
            _canShoot = true;
        }
        
        private IEnumerator SetDefaultColor(float colorCD)
        {
            yield return new WaitForSeconds(colorCD);
            _spriteRenderer.color = _baseColor;
        }
     
        private IEnumerator KnockBackRoutine()
        {
            yield return new WaitForSeconds(0.5f);
            _rigidbody.velocity = Vector2.zero;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("PlayerBody") && _contactDamage)
            {
                _target.GetKnockedBack(transform, _knockBackPower);
                _target.UpdateHealth(-1);
            }
        }
    }
}