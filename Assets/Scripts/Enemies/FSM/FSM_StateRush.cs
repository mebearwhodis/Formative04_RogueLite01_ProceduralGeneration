﻿using UnityEngine;

namespace FSM
{
    public class FSM_StateRush : FSM_IState
    {
        private FSM_Enemy _entity;
        private Rigidbody2D _rb;
        private Vector2 _direction;

        public FSM_StateRush(FSM_Enemy entity)
        {
            _entity = entity;
            _rb = _entity.GetComponent<Rigidbody2D>();
        }
        
        public void OnUpdate()
        {
            _entity.transform.Translate(_direction * (_entity.RushSpeed * Time.deltaTime));
        }

        public void OnEnter()
        {
            Debug.Log("Rush state entered!");
            // Calculate direction towards player
            _direction = (GameManager.Instance.GetPlayerPosition() - _entity.transform.position).normalized;
        }

        public void OnExit()
        {
            Debug.Log("Rush state exited!");
            _rb.velocity = Vector2.zero;
        }
    }
}