using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraSetter : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Room"))
        {
            Debug.Log("Entered Room");
            other.gameObject.transform.parent.parent.GetComponentInChildren<CinemachineVirtualCamera>().m_Priority = 11;
            //StartCoroutine(ChangeRoom(other));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Room"))
            other.gameObject.transform.parent.parent.GetComponentInChildren<CinemachineVirtualCamera>().m_Priority = 0;
    }

    IEnumerator ChangeRoom(Collider2D other)
    {
        Time.timeScale = 0;
        //Do stuff maybe
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1;
    }
}
