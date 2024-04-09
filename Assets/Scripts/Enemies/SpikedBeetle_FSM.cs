using System;
using System.Collections;
using FSM;
using UnityEngine;
using UnityEngine.Serialization;

public class SpikedBeetle_FSM : MonoBehaviour
{
    private FSM_StateMachine _stateMachine;
    private FSM_StateWander _wanderState;
    private FSM_StateRush _rushState;
    private FSM_StateVulnerable _vulnerableState;
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
        _rushState = new FSM_StateRush(_thisEntity);
        _vulnerableState = new FSM_StateVulnerable();
        
        _stateMachine.ChangeState(_wanderState);
        _stateMachine.AddTransition(_wanderState, _rushState, SeePlayer);
        _stateMachine.AddTransition(_rushState, _vulnerableState, () => CheckCollisionWithTag("Shield"));
        _stateMachine.AddTransition(_rushState, _wanderState, () => CheckCollisionWithTag("Wall"));
        _stateMachine.AddTransition(_vulnerableState, _wanderState, StopVulnerable);
    }

    private void Update()
    {
        _stateMachine.UpdateState();
    }

    private bool SeePlayer()
    {
        _lastTagHit = null;
        var targetPosition = _target.transform.position;
        var thisPosition = transform.position;
        return Math.Abs(targetPosition.x - thisPosition.x) < 0.5f || Math.Abs(targetPosition.y - thisPosition.y) < 0.5f;
    }

    private bool StopVulnerable()
    {
        return !_vulnerable;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            _lastTagHit = "Wall";
            Debug.Log("Trigger Wall hit");
        }
        else if (other.gameObject.CompareTag("Shield"))
        {
            _lastTagHit = "Shield";
            _vulnerable = true;
            Debug.Log("Trigger Shield hit");
            StartCoroutine("VulnerableTime");
        }
    }

    private IEnumerator VulnerableTime()
    {
        yield return new WaitForSeconds(4);
        _vulnerable = false;
    }

    private bool CheckCollisionWithTag(string checkTag)
    {
        return checkTag == _lastTagHit;
    }
}