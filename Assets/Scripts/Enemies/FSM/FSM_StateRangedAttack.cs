using UnityEngine;

namespace Enemies.FSM
{
    public class FSM_StateRangedAttack : FSM_IState
    {
        private FSM_Enemy _entity;
        private Rigidbody2D _rigidbody;

        public FSM_StateRangedAttack(FSM_Enemy entity)
        {
            _entity = entity;
            _rigidbody = _entity.Rigidbody;
        }

        public void OnUpdate()
        {
            _entity.ThrowProjectile();
        }

        public void OnEnter()
        {
            _rigidbody.velocity = Vector2.zero;
        }

        public void OnExit()
        {
        }
    }
}