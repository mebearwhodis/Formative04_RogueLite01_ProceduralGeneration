using System.Collections;
using Cinemachine;
using UnityEngine;

public class CameraSetter : MonoBehaviour
{
    private CinemachineConfiner2D _confiner2D;
    
    // Start is called before the first frame update
    void Start()
    {
        _confiner2D = GetComponent<CinemachineConfiner2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Room"))
        {
            Debug.Log("Entered Room");
            _confiner2D.m_BoundingShape2D = other.gameObject.GetComponentInParent<CompositeCollider2D>();
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
