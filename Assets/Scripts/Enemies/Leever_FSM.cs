using System;
using FSM;
using UnityEngine;

public class Leever_FSM : MonoBehaviour
{
    private FSM_StateMachine _stateMachine;
    private FSM_StateWander _wanderState;
    private FSM_StateChase _chaseState;
    private FSM_StateHide _hideState;
    private FSM_Enemy _thisEntity;
    
    private PlayerController _target;
    //private Vector2 _targetPosition
    //private Vector2 _direction;

    private string _lastTagHit;
    private bool _vulnerable;

    private void Start()
    {
        _target = PlayerController.Instance;
        _thisEntity = GetComponent<FSM_Enemy>();

        _stateMachine = new FSM_StateMachine();
        _wanderState = new FSM_StateWander(_thisEntity);
        _hideState = new FSM_StateHide(_thisEntity);
        
        _stateMachine.ChangeState(_hideState);
        //_stateMachine.AddTransition(_hideState, _chaseState, SeePlayer);
        //_stateMachine.AddTransition(_chaseState, _wanderState, );
    }

    private void Update()
    {
        _stateMachine.UpdateState();
    }

    private bool SeePlayer()
    {
        //either raycast if obstacles or check in a radius
        var targetPosition = _target.transform.position;
        var thisPosition = transform.position;
        return Math.Abs(targetPosition.x - thisPosition.x) < 0.5f || Math.Abs(targetPosition.y - thisPosition.y) < 0.5f;
    }

}