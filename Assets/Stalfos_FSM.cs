using System.Collections;
using System.Collections.Generic;
using FSM;
using UnityEngine;

public class Stalfos_FSM : MonoBehaviour
{
    private FSM_StateMachine _stateMachine;
    private FSM_StateWander _wanderState;
    private FSM_StateChase _chaseState;
    private FSM_StateRangedAttack _rangedAttackState;
    private FSM_Enemy _thisEntity;
    
    void Start()
    {
        _thisEntity = GetComponent<FSM_Enemy>();
        
        _stateMachine = new FSM_StateMachine();
        _chaseState = new FSM_StateChase(_thisEntity);
        _rangedAttackState = new FSM_StateRangedAttack(_thisEntity);
        
        _stateMachine.ChangeState(_chaseState);
        _stateMachine.AddTransition(_chaseState, _rangedAttackState, () => InAttackRange(true));
        _stateMachine.AddTransition(_rangedAttackState, _chaseState, () => InAttackRange(false));
    }

    void Update()
    {
        _stateMachine.UpdateState();
    }

    private bool InAttackRange(bool target)
    {
        var targetPos = _thisEntity.Target.transform.position;
        var thisPos = _thisEntity.transform.position;
        
        bool isInAttackRange = (Mathf.Abs((targetPos - thisPos).magnitude) >
                                _thisEntity.MinAttackDistance &&
                                Mathf.Abs((targetPos - thisPos).magnitude) <
                                _thisEntity.MaxAttackDistance);
        
        return target == isInAttackRange;
    }
}
