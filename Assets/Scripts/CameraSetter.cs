using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraSetter : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineConfiner2D _confiner2D;
    
    // Start is called before the first frame update
    void Start()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _confiner2D = GetComponent<CinemachineConfiner2D>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Room"))
        {
            Debug.Log("Entered Room");
            _confiner2D.m_BoundingShape2D = other.gameObject.GetComponentInParent<CompositeCollider2D>();
            // _virtualCamera.m_LookAt = other.gameObject.transform.parent.parent.GetComponentInChildren<Center>().transform;
            // _virtualCamera.m_Follow = other.gameObject.transform.parent.parent.GetComponentInChildren<Center>().transform;
            //StartCoroutine(ChangeRoom(other));
        }
    }

    IEnumerator ChangeRoom(Collider2D other)
    {
        Time.timeScale = 0;
        //_confiner2D.m_BoundingShape2D = other.gameObject.GetComponentInParent<CompositeCollider2D>();
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1;
    }
}
