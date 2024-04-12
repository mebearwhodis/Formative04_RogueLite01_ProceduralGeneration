using UnityEngine;

namespace FSM
{
    public class FSM_StateHide : FSM_IState
    {
        private FSM_Enemy _entity;
        private Rigidbody2D _rigidbody;

        public FSM_StateHide(FSM_Enemy entity)
        {
            _entity = entity;
            _rigidbody = _entity.Rigidbody;
        }
        
        public void OnUpdate()
        {
            
        }

        public void OnEnter()
        {
            _rigidbody.velocity = Vector2.zero;
            _entity.Animator.SetTrigger("goingUnder");
        }

        public void OnExit()
        {
            Vector2 spawnPosition = _entity.SpawnPoint;
            _entity.transform.position = spawnPosition + Random.insideUnitCircle * _entity.PopoutRadius;
            _entity.Animator.SetTrigger("comingOut");
        }
    }
}