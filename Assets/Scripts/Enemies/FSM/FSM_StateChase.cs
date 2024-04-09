using UnityEngine;

namespace FSM
{
    public class FSM_StateChase : FSM_IState
    {
        private FSM_Enemy _entity;
        private Rigidbody2D _rigidbody;

        public FSM_StateChase(FSM_Enemy entity)
        {
            _entity = entity;
            _rigidbody = _entity.Rigidbody;
        }
        
        public void OnUpdate()
        {
            
            Vector2 velocity = _rigidbody.velocity;
            Vector2 acceleration = Vector2.zero;

            // Calculate desired velocity towards the target
            Vector2 desiredVelocity = (_entity.Target.transform.position - _entity.transform.position);
            
            // Calculate distance to target
            float distanceToTarget = Vector2.Distance(_entity.Target.transform.position, _entity.transform.position);

            // Adjust desired velocity based on target distance
            float targetDistance = _entity.TargetDistance;
            if (distanceToTarget < targetDistance)
            {
                desiredVelocity *= -1; // Reverse direction if closer than target distance
            }

            // Update acceleration and velocity
            acceleration += desiredVelocity - velocity;
            velocity += acceleration * Time.deltaTime;

            // Limit velocity to chase speed
            if (velocity.magnitude > _entity.ChaseSpeed)
            {
                velocity = velocity.normalized * _entity.ChaseSpeed;
            }

            // Apply velocity to rigidbody
            _rigidbody.velocity = velocity;

            // Vector2 direction = (GameManager.Instance.GetPlayerPosition() - _entity.transform.position).normalized;
            // _entity.transform.Translate(direction * (_entity.ChaseSpeed * Time.deltaTime));
        }

        public void OnEnter()
        {
            Debug.Log("Chase state entered!");
        }

        public void OnExit()
        {
            Debug.Log("Chase state exited!");
        }
    }
}