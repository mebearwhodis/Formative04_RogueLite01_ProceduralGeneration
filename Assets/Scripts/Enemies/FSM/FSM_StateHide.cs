using UnityEngine;

namespace FSM
{
    public class FSM_StateHide : FSM_IState
    {
        private FSM_Enemy _entity;
        private Rigidbody2D _rigidbody;
        private SpriteRenderer _spriteRenderer;
        private bool _hidden = true;

        public FSM_StateHide(FSM_Enemy entity)
        {
            _entity = entity;
            _rigidbody = _entity.Rigidbody;
            _spriteRenderer = _entity.SpriteRenderer;
        }
        
        public void OnUpdate()
        {
            
        }

        public void OnEnter()
        {
            _spriteRenderer.enabled = false;
        }

        public void OnExit()
        {
            _spriteRenderer.enabled = true;
            _hidden = false;
        }
    }
}