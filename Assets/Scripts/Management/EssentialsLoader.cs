using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialsLoader : MonoBehaviour
{
    
    [SerializeField] private DungeonGeneratorV2 _dungeonGeneratorV2;
    [SerializeField] private GameObject _player;
    PlayerController player;

    public void PlacePlayer()
    {
        if (PlayerController.Instance == null)
        {
            player = Instantiate(_player).GetComponent<PlayerController>();
        }
        else
        {
            player = PlayerController.Instance;
        }

        // Can place in any scene to set the spawn point of our hero in that scene
        if (FindFirstObjectByType<PlayerSpawn>())
        {
            player.transform.position = FindFirstObjectByType<PlayerSpawn>().transform.position;
            Debug.Log("Moving player to spawn point");
        }
        else
        {
            Debug.Log("Nowhere to put player");
            //player.transform.position = new Vector3((float)_dungeonGeneratorV2.RoomSize.x / 2, (float)_dungeonGeneratorV2.RoomSize.y / 2, 0);
        }
    }
}