using Cinemachine;
using UnityEngine;

public class IslandCamera : MonoBehaviour
{
    private CinemachineVirtualCamera _camera;

    private void Start()
    {
        _camera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
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