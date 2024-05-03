using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class IslandCamera : MonoBehaviour
{
    private CinemachineVirtualCamera _camera;
    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<CinemachineVirtualCamera>();
    }
    
    // Update is called once per frame
    void Update()
    {
        //if(_camera.m_Follow is not null && _camera.m_LookAt is not null){return;}
        //else
        {
            if (PlayerController.Instance is not null)
            {
                _camera.m_Follow = PlayerController.Instance.transform;
                _camera.m_LookAt = PlayerController.Instance.transform;
            }
            else
            {
                _camera.m_Follow = null;
                _camera.m_LookAt = null;
            }
        }
    }
}
