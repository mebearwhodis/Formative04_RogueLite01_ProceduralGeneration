using System;
using System.Collections;
using FSM;
using UnityEngine;
using Random = UnityEngine.Random;

public class Leever_FSM : MonoBehaviour
{
    private PlayerController _target;
    private FSM_Enemy _thisEntity;

    private FSM_StateMachine _stateMachine;

    private FSM_StateChase _chaseState;
    private FSM_StateHide _hideState;


    private bool _hidden;
    private bool _canSwitch = true;

    private void Start()
    {
        _target = PlayerController.Instance;
        _thisEntity = GetComponent<FSM_Enemy>();

        _stateMachine = new FSM_StateMachine();
        _chaseState = new FSM_StateChase(_thisEntity);
        _hideState = new FSM_StateHide(_thisEntity);

        _stateMachine.ChangeState(_chaseState);
        _stateMachine.AddTransition(_hideState, _chaseState, () => !_hidden);
        _stateMachine.AddTransition(_chaseState, _hideState, () => _hidden);
    }

    private void Update()
    {
        _stateMachine.UpdateState();
        if (!_canSwitch) { return; }
        StartCoroutine(SwitchHiddenStatus());
    }

    private void ToggleSpriteRenderer()
    {
        _thisEntity.SpriteRenderer.enabled = !_hidden;
    }

    private IEnumerator SwitchHiddenStatus()
    {
        _canSwitch = false; // Prevent switching while coroutine is running
        float randomTimer = Random.Range(4f, 6f);
        yield return new WaitForSeconds(randomTimer);
        _hidden = !_hidden; // Toggle hidden status
        _canSwitch = true; // Allow switching again
    }
}