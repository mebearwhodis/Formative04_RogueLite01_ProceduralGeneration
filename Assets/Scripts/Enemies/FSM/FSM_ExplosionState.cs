using UnityEngine;

namespace Enemies.FSM
{
    public class FSM_ExplosionState : FSM_IState
    {
        private FSM_Enemy _entity;
        private Rigidbody2D _rigidbody;
        private Animator _animator;
        private bool _targetInExplosionRadius = false;
        private static readonly int Explode = Animator.StringToHash("explode");

        public FSM_ExplosionState(FSM_Enemy entity)
        {
            _entity = entity;
            _rigidbody = _entity.Rigidbody;
            _animator = _entity.Animator;
        }

        public void OnUpdate()
        {
            //Calculate desired velocity towards the target
            Vector2 desiredVelocity = (_entity.Target.transform.position - _entity.transform.position);

            //Calculate distance to target
            float distanceToTarget = desiredVelocity.magnitude;

            //Adjust desired velocity based on target distance
            float targetDistance = _entity.ExplosionRadius;
            if (distanceToTarget <= targetDistance)
            {
                _targetInExplosionRadius = true;
                _rigidbody.velocity = Vector2.zero;
            }

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
            _entity.Animator.SetTrigger(Explode);
        }

        public void OnExit()
        {
        }
    }
}