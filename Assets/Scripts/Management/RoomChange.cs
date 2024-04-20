using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomChange : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Room"))
        {
            Debug.Log("Entered Room");
            //Set current room's camera as the main one
            other.gameObject.transform.parent.parent.GetComponentInChildren<CinemachineVirtualCamera>().m_Priority = 11;
            
            //StartCoroutine(ChangeRoom(other));
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Room"))
        {
            Bounds feetBounds = this.GetComponent<BoxCollider2D>().bounds;
            Bounds tilemapBounds = other.GetComponent<TilemapCollider2D>().bounds;
            
            //If the Feet collider is entirely contained in the room,
            if (tilemapBounds.Contains(feetBounds.min) && tilemapBounds.Contains(feetBounds.max))
            {
                RoomSpawn room = other.GetComponentInParent<RoomSpawn>();
                //Close the doors if the room hasn't been completed yet
                if (!room.completed)
                {
                    room.open = false;
                    room.ToggleDoors(true);
                }
                //Otherwise open (or keep open) the doors
                if (room.completed)
                {
                    room.open = true;
                    room.ToggleDoors(false);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //Reset the room's camera
        if(other.gameObject.CompareTag("Room"))
            other.gameObject.transform.parent.parent.GetComponentInChildren<CinemachineVirtualCamera>().m_Priority = 1;
    }

    IEnumerator ChangeRoom(Collider2D other)
    {
        GameManager.Instance.SetPause();
        Debug.Log("Coroutine started");
        //Do stuff maybe
        yield return new WaitForSecondsRealtime(1f);
        GameManager.Instance.SetPause();
    }
}
