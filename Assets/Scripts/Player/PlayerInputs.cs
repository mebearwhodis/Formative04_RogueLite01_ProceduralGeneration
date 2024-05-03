using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    private Vector2 _moveValue = new Vector2(0, 0);
    
    private bool _arrowShot = false;

    public Vector2 MoveValue => _moveValue;

    public bool ArrowShot => _arrowShot;

    private void OnMove(InputValue value)
    {
        _moveValue = value.Get<Vector2>();
    }

    private void OnShoot(InputValue value)
    {
        _arrowShot = value.isPressed;
    }
}