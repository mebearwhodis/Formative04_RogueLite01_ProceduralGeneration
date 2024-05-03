using UnityEngine;

public class EssentialsLoader : MonoBehaviour
{
    
    [SerializeField] private DungeonGeneratorV2 _dungeonGeneratorV2;
    [SerializeField] private GameObject _player;
    private PlayerController _playerController;

    public void PlacePlayer()
    {
        if (PlayerController.Instance == null)
        {
            _playerController = Instantiate(_player).GetComponent<PlayerController>();
        }
        else
        {
            _playerController = PlayerController.Instance;
        }

        //Can place in any scene to set the spawn point of our hero in that scene
        if (FindFirstObjectByType<PlayerSpawn>())
        {
            _playerController.transform.position = FindFirstObjectByType<PlayerSpawn>().transform.position;
        }
    }
}