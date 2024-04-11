using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace FSM
{
    public class FSM_Enemy : MonoBehaviour
    {
        private Animator _animator;
        private Rigidbody2D _rigidbody;
        private SpriteRenderer _spriteRenderer;
        private PlayerController _target = PlayerController.Instance;

        public Animator Animator => _animator;
        public Rigidbody2D Rigidbody => _rigidbody;
        public SpriteRenderer SpriteRenderer => _spriteRenderer;
        public PlayerController Target => _target;

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
        }

        public void ThrowProjectile()
        {
            if(!_canShoot){return;}
            StartCoroutine("RangedAttack");
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
    }
}