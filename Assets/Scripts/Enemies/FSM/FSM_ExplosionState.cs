using UnityEngine;

namespace FSM
{
    public class FSM_ExplosionState : FSM_IState
    {
        private FSM_Enemy _entity;
        private Rigidbody2D _rigidbody;
        private Animator _animator;
        private bool _targetInExplosionRadius = false;
        
        public FSM_ExplosionState(FSM_Enemy entity)
        {
            _entity = entity;
            _rigidbody = _entity.Rigidbody;
            _animator = _entity.Animator;
        }
        
        public void OnUpdate()
        {
            //Chase but slower ? or faster and stop when nearing the end ?
            
            
            //Chase Update:
            Vector2 velocity = _rigidbody.velocity;
            Vector2 acceleration = Vector2.zero;
            
            // Calculate desired velocity towards the target
            Vector2 desiredVelocity = (_entity.Target.transform.position - _entity.transform.position);
            
            // Calculate distance to target
            float distanceToTarget = desiredVelocity.magnitude;
            
            // Adjust desired velocity based on target distance
            float targetDistance = _entity.ExplosionRadius;
            if (distanceToTarget <= targetDistance)
            {
                _targetInExplosionRadius = true;
                _rigidbody.velocity = Vector2.zero;
            }
            
            // // Update acceleration and velocity
            // acceleration += desiredVelocity - velocity;
            // velocity += acceleration * Time.deltaTime;
            //
            // // Limit velocity to chase speed
            // if (velocity.magnitude > _entity.ChaseSpeed)
            // {
            //     velocity = velocity.normalized * _entity.ChaseSpeed;
            // }
            //
            // // Apply velocity to rigidbody
            // _rigidbody.velocity = velocity;
            if (_targetInExplosionRadius)
            {
                Vector2 direction = (GameManager.Instance.GetPlayerPosition() - _entity.transform.position).normalized;
                _entity.transform.Translate(direction * (_entity.ExplosionChaseSpeed * Time.deltaTime));
            }
            else
            {
                Vector2 direction = (GameManager.Instance.GetPlayerPosition() - _entity.transform.position).normalized;
                _entity.transform.Translate(direction * (_entity.ChaseSpeed * Time.deltaTime));
            }
        }

        public void OnEnter()
        {
            Debug.Log("Explosion state entered!");
            _entity.Animator.SetTrigger("explode");
        }

        public void OnExit()
        {
            Debug.Log("Explosion state exited!");
        }
    }
}