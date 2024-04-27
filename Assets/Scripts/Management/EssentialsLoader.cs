using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialsLoader : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private DungeonGeneratorV2 _dungeonGeneratorV2;
    
    private GameObject cameraContainer;

    private void Start()
    {
        if(PlayerController.Instance == null) {
            PlayerController clone = Instantiate(_player).GetComponent<PlayerController>();

            // Can place in any scene to set the spawn point of our hero in that scene
            if (FindFirstObjectByType<PlayerSpawn>()) {
                clone.transform.position = FindFirstObjectByType<PlayerSpawn>().transform.position;
            } else {
                clone.transform.position = new Vector3((float)_dungeonGeneratorV2.RoomSize.x / 2, (float)_dungeonGeneratorV2.RoomSize.y / 2, 0);
                //clone.transform.position = new Vector3((float)1 / 2, (float)1 / 2, 0);
            }
        }

        // if(CameraController.Instance == null) {
        //     Instantiate(cameraContainer).GetComponent<CameraController>();
        // }
    }
}