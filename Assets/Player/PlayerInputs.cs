using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    
    public Vector2 _moveValue = new Vector2(0,0);
    
    public bool _itemUsed = false;
    public bool _positionReset = false;

    

    public void OnMove(InputValue value)
    {
        _moveValue = value.Get<Vector2>();
    }

    public void OnUse(InputValue value)
    {
        _itemUsed = value.isPressed;
    }

    public void OnReset(InputValue value)
    {
        _positionReset = value.isPressed;
    }
}
