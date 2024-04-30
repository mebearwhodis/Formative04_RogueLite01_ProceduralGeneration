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
            _camera.m_Follow = FindFirstObjectByType<PlayerController>().transform;
            _camera.m_LookAt = FindFirstObjectByType<PlayerController>().transform;
        }
    }
}
