using System;
using System.Collections;
using FSM;
using UnityEngine;

public class Bombite_FSM : MonoBehaviour
{
    private FSM_StateMachine _stateMachine;
    private FSM_StateWander _wanderState;
    private FSM_StateChase _chaseState;
    private FSM_ExplosionState _explosionState;
    private FSM_Enemy _thisEntity;
    
    private PlayerController _target;


    private void Start()
    {
        _target = PlayerController.Instance;
        _thisEntity = GetComponent<FSM_Enemy>();

        _stateMachine = new FSM_StateMachine();
        // _wanderState = new FSM_StateWander(_thisEntity);
        _chaseState = new FSM_StateChase(_thisEntity);
        _explosionState = new FSM_ExplosionState(_thisEntity);
        
        _stateMachine.ChangeState(_chaseState);
        // _stateMachine.AddTransition(_wanderState, _chaseState, SeePlayer);
        _stateMachine.AddTransition(_chaseState, _explosionState, PlayerInRange);
    }

    private void Update()
    {
        _stateMachine.UpdateState();
    }
    //
    // private bool SeePlayer()
    // {
    //     //either raycast if obstacles or check in a radius
    //     return false;
    // }

    private bool PlayerInRange()
    {
        var targetPos = _thisEntity.Target.transform.position;
        var thisPos = _thisEntity.transform.position;
       
        bool isInExplosionRadius = (targetPos - thisPos).magnitude <= _thisEntity.ExplosionRadius;
        
        return isInExplosionRadius;
    }

    private void BombiteExplosion()
    {
        StartCoroutine("Explode");
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(1);
        //Explosion code, destroy enemy, etc.
        Debug.Log("Exploded :(");
        PlayerController.Instance.CurrentRoom.monstersLeft--;
        Destroy(this.gameObject);
    }
}
