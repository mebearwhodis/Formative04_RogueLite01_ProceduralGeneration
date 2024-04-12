using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    
    private Vector2 _moveValue = new Vector2(0,0);
    
    private bool _itemUsed = false;
    private bool _positionReset = false;
    private bool _bombDropped = false;
    private bool _arrowShot = false;

    public Vector2 MoveValue => _moveValue;

    public bool ArrowShot => _arrowShot;

    public bool BombDropped => _bombDropped;


    private void OnMove(InputValue value)
    {
        _moveValue = value.Get<Vector2>();
    }

    private void OnUse(InputValue value)
    {
        _itemUsed = value.isPressed;
    }

    private void OnReset(InputValue value)
    {
        _positionReset = value.isPressed;
    }

    private void OnShoot(InputValue value)
    {
        _arrowShot = value.isPressed;
    }

    private void OnBomb(InputValue value)
    {
        _bombDropped = value.isPressed;
    }
}
