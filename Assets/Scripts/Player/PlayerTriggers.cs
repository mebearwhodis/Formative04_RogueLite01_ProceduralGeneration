using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class PlayerTriggers : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Room"))
        {
            Debug.Log("Entered Room");
            //Set current room's camera as the main one
            other.gameObject.transform.parent.parent.GetComponentInChildren<CinemachineVirtualCamera>().m_Priority = 11;
        }

        if (other.gameObject.CompareTag("Entrance"))
        {
            GameManager.Instance.SetGameState(GameManager.GameState.DungeonState);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Room"))
        {
            PlayerController.Instance.CurrentRoom = other.GetComponentInParent<RoomSpawn>();
            Bounds feetBounds = this.GetComponent<BoxCollider2D>().bounds;
            Bounds tilemapBounds = other.GetComponent<TilemapCollider2D>().bounds;

            //If the Feet collider is entirely contained in the room,
            if (tilemapBounds.Contains(feetBounds.min) && tilemapBounds.Contains(feetBounds.max))
            {
                RoomSpawn room = other.GetComponentInParent<RoomSpawn>();
                //Close the doors if the room hasn't been completed yet
                if (!room.completed && room.open)
                {
                    room.open = false;
                    room.ToggleDoors(true);
                    SpawnMonsters(room);
                }

                //If there are no monsters left in the room, mark it as completed
                if (room.monstersLeft == 0)
                {
                    room.completed = true;
                }

                //Otherwise open (or keep open) the doors
                if (room.completed && !room.open)
                {
                    room.open = true;
                    room.ToggleDoors(false);
                }
            }
        }
    }

    private void SpawnMonsters(RoomSpawn room)
    {
        EnemySpawner spawnPoint = room.GetComponentInChildren<EnemySpawner>();
        if (room.type == Room.RoomType.Combat)
        {
            int currentCost = 0;
            do
            {
                int randomMonster = Random.Range(0, 4);
                currentCost += spawnPoint.SpawnMonster(randomMonster);
                room.monstersLeft++;
            } while (currentCost < room.maxDifficulty);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //Reset the room's camera
        if (other.gameObject.CompareTag("Room"))
            other.gameObject.transform.parent.parent.GetComponentInChildren<CinemachineVirtualCamera>().m_Priority = 1;
    }
}