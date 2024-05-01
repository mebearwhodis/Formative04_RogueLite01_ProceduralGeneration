using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomSpawn : MonoBehaviour
{
    [SerializeField] private Tilemap _wallsMap;
    [SerializeField] private Tilemap _floorMap;
    [SerializeField] private Tilemap _doorsMap;
    [SerializeField] private Tilemap _roomBoundaries;
    [SerializeField] private TileBase _wallTile;
    [SerializeField] private TileBase _floorTile;
    [SerializeField] private TileBase _flairFloorTile;
    [SerializeField] private TileBase _closedDoorTile;
    [SerializeField] private TileBase _openDoorTile;
    [SerializeField] private TileBase _blockTile;
    [SerializeField] private TileBase _holeTile;
    [SerializeField] private EnemySpawner _center;

    public Vector2Int size;

    public bool up, down, left, right;
    public Room.RoomType type;
    public int spacesFromStart;
    public int maxDifficulty;
    public int monstersLeft;
    public bool completed;
    public bool open;

    private void Start()
    {
        DrawRoom();
        CreateDoors();
        SetCenter();
        FillRoom();
        SetCameraPriority();
    }

    private void DrawRoom()
    {
        for (int x = -2; x <= size.x + 1; x++)
        {
            for (int y = -2; y <= size.y + 1; y++)
            {
                if (x <= -1 || x >= size.x || y <= -1 || y >= size.y)
                {
                    _wallsMap.SetTile(new Vector3Int(x, y, 0), _wallTile);
                    _roomBoundaries.SetTile(new Vector3Int(x, y, 0), _wallTile);
                    if (x == -2 || x == size.x + 1 || y == -2 || y == size.y + 1)
                    {
                        _wallsMap.SetColor(new Vector3Int(x, y, 0), Color.clear);
                    }
                }
                else
                {
                    _floorMap.SetTile(new Vector3Int(x, y, 0), _floorTile);
                    _roomBoundaries.SetTile(new Vector3Int(x, y, 0), _wallTile);
                }
            }
        }

        _roomBoundaries.color = Color.clear;
        GenerateRoomDecorations();
    }

    private void GenerateRoomDecorations()
    {
        // Define the total number of floor tiles in the room
        int totalFloorTiles = (size.x - 4) * (size.y - 4);

        // Calculate the number of flair tiles (30% of total floor tiles)
        int numFlairTiles = Mathf.RoundToInt(totalFloorTiles * 0.3f);

        // Randomly replace some floor tiles with flair tiles
        for (int i = 0; i < numFlairTiles; i++)
        {
            int x = UnityEngine.Random.Range(0, size.x - 4) + 2;
            int y = UnityEngine.Random.Range(0, size.y - 4) + 2;
            _floorMap.SetTile(new Vector3Int(x, y, 0), _flairFloorTile);
        }

        // Calculate the number of block tiles (10% of total floor tiles)
        int numBlockTiles = Mathf.RoundToInt(totalFloorTiles * 0.1f);

        // Randomly place block tiles
        for (int i = 0; i < numBlockTiles; i++)
        {
            int x = UnityEngine.Random.Range(1, size.x - 1);
            int y = UnityEngine.Random.Range(1, size.y - 1);

            // Check if the tile is not next to a wall or another block tile
            if (
                _blockTile != null && 
                _wallsMap.GetTile(new Vector3Int(x, y, 0)) == null &&
                _wallsMap.GetTile(new Vector3Int(x, y + 1, 0)) == null &&
                _wallsMap.GetTile(new Vector3Int(x + 1, y + 1, 0)) == null &&
                _wallsMap.GetTile(new Vector3Int(x + 1, y, 0)) == null &&
                _wallsMap.GetTile(new Vector3Int(x + 1, y - 1, 0)) == null &&
                _wallsMap.GetTile(new Vector3Int(x, y - 1, 0)) == null &&
                _wallsMap.GetTile(new Vector3Int(x - 1, y - 1, 0)) == null &&
                _wallsMap.GetTile(new Vector3Int(x - 1, y, 0)) == null &&
                _wallsMap.GetTile(new Vector3Int(x - 1, y + 1, 0)) == null
                )
            {
                _wallsMap.SetTile(new Vector3Int(x, y, 0), _blockTile);
            }
        }

        // Calculate the number of hole tiles (10% of total floor tiles)
        int numHoleTiles = Mathf.RoundToInt(totalFloorTiles * 0.1f);

        // Randomly place hole tiles
        for (int i = 0; i < numHoleTiles; i++)
        {
            int x = UnityEngine.Random.Range(1, size.x - 1);
            int y = UnityEngine.Random.Range(1, size.y - 1);

            // Check if the tile is not next to a wall or another hole tile
            if (
                _holeTile != null && 
                _wallsMap.GetTile(new Vector3Int(x, y, 0)) == null &&
                _wallsMap.GetTile(new Vector3Int(x, y + 1, 0)) == null &&
                _wallsMap.GetTile(new Vector3Int(x + 1, y + 1, 0)) == null &&
                _wallsMap.GetTile(new Vector3Int(x + 1, y, 0)) == null &&
                _wallsMap.GetTile(new Vector3Int(x + 1, y - 1, 0)) == null &&
                _wallsMap.GetTile(new Vector3Int(x, y - 1, 0)) == null &&
                _wallsMap.GetTile(new Vector3Int(x - 1, y - 1, 0)) == null &&
                _wallsMap.GetTile(new Vector3Int(x - 1, y, 0)) == null &&
                _wallsMap.GetTile(new Vector3Int(x - 1, y + 1, 0)) == null
            )
            {
                _wallsMap.SetTile(new Vector3Int(x, y, 0), _holeTile);
            }
        }
    }
    

    private void CreateDoors()
    {
        if (up)
        {
            _wallsMap.SetTile(new Vector3Int(size.x / 2, size.y, 0), null);
            _wallsMap.SetTile(new Vector3Int(size.x / 2, size.y + 1, 0), null);
            _doorsMap.SetTile(new Vector3Int(size.x / 2, size.y, 0), _openDoorTile);

            _doorsMap.SetTile(new Vector3Int(size.x / 2 - 1, size.y, 0), _closedDoorTile);
            _doorsMap.SetTile(new Vector3Int(size.x / 2 + 1, size.y, 0), _closedDoorTile);
            _doorsMap.SetTile(new Vector3Int(size.x / 2 - 1, size.y + 1, 0), _closedDoorTile);
            _doorsMap.SetTile(new Vector3Int(size.x / 2 + 1, size.y + 1, 0), _closedDoorTile);
        }

        if (down)
        {
            _wallsMap.SetTile(new Vector3Int(size.x / 2, -1, 0), null);
            _wallsMap.SetTile(new Vector3Int(size.x / 2, -2, 0), null);
            _doorsMap.SetTile(new Vector3Int(size.x / 2, -1, 0), _openDoorTile);

            _doorsMap.SetTile(new Vector3Int(size.x / 2 - 1, -2, 0), _closedDoorTile);
            _doorsMap.SetTile(new Vector3Int(size.x / 2 + 1, -2, 0), _closedDoorTile);
            _doorsMap.SetTile(new Vector3Int(size.x / 2 - 1, -1, 0), _closedDoorTile);
            _doorsMap.SetTile(new Vector3Int(size.x / 2 + 1, -1, 0), _closedDoorTile);
        }

        if (left)
        {
            _wallsMap.SetTile(new Vector3Int(-1, size.y / 2, 0), null);
            _wallsMap.SetTile(new Vector3Int(-2, size.y / 2, 0), null);
            _doorsMap.SetTile(new Vector3Int(-1, size.y / 2, 0), _openDoorTile);


            _doorsMap.SetTile(new Vector3Int(-2, size.y / 2 - 1, 0), _closedDoorTile);
            _doorsMap.SetTile(new Vector3Int(-2, size.y / 2 + 1, 0), _closedDoorTile);
            _doorsMap.SetTile(new Vector3Int(-1, size.y / 2 - 1, 0), _closedDoorTile);
            _doorsMap.SetTile(new Vector3Int(-1, size.y / 2 + 1, 0), _closedDoorTile);
        }

        if (right)
        {
            _wallsMap.SetTile(new Vector3Int(size.x, size.y / 2, 0), null);
            _wallsMap.SetTile(new Vector3Int(size.x + 1, size.y / 2, 0), null);
            _doorsMap.SetTile(new Vector3Int(size.x, size.y / 2, 0), _openDoorTile);

            _doorsMap.SetTile(new Vector3Int(size.x, size.y / 2 - 1, 0), _closedDoorTile);
            _doorsMap.SetTile(new Vector3Int(size.x, size.y / 2 + 1, 0), _closedDoorTile);
            _doorsMap.SetTile(new Vector3Int(size.x + 1, size.y / 2 - 1, 0), _closedDoorTile);
            _doorsMap.SetTile(new Vector3Int(size.x + 1, size.y / 2 + 1, 0), _closedDoorTile);
        }
    }

    public void ToggleDoors(bool state)
    {
        _doorsMap.GetComponent<TilemapCollider2D>().enabled = state;
        Debug.Log("Toggled doors: " + state);
        if (_doorsMap.GetComponent<TilemapCollider2D>().enabled == true)
        {
            if (up)
            {
                _doorsMap.SetTile(new Vector3Int(size.x / 2, size.y, 0), _closedDoorTile);
            }

            if (down)
            {
                _doorsMap.SetTile(new Vector3Int(size.x / 2, -1, 0), _closedDoorTile);
            }

            if (left)
            {
                _doorsMap.SetTile(new Vector3Int(-1, size.y / 2, 0), _closedDoorTile);
            }

            if (right)
            {
                _doorsMap.SetTile(new Vector3Int(size.x, size.y / 2, 0), _closedDoorTile);
            }
        }

        if (_doorsMap.GetComponent<TilemapCollider2D>().enabled == false)
        {
            if (up)
            {
                _doorsMap.SetTile(new Vector3Int(size.x / 2, size.y, 0), _openDoorTile);
            }

            if (down)
            {
                _doorsMap.SetTile(new Vector3Int(size.x / 2, -1, 0), _openDoorTile);
            }

            if (left)
            {
                _doorsMap.SetTile(new Vector3Int(-1, size.y / 2, 0), _openDoorTile);
            }

            if (right)
            {
                _doorsMap.SetTile(new Vector3Int(size.x, size.y / 2, 0), _openDoorTile);
            }
        }
    }

    private void SetCenter()
    {
        _center.transform.localPosition = new Vector3((float)size.x / 2, (float)size.y / 2, 0);
    }

    private void FillRoom()
    {
        switch (type)
        {
            case Room.RoomType.NA:
                Debug.Log("A room is missing a type");
                break;
            case Room.RoomType.Start:
                _center.gameObject.SetActive(true);
                completed = true;
                open = true;
                break;
            case Room.RoomType.Treasure:
                completed = true;
                open = true;
                break;
            case Room.RoomType.Shop:
                completed = true;
                open = true;
                break;
            case Room.RoomType.Combat:
                if (spacesFromStart * spacesFromStart + 5 > 30)
                {
                    maxDifficulty = 30;
                }
                else
                {
                    maxDifficulty = spacesFromStart * spacesFromStart + 5;
                }

                break;
            case Room.RoomType.Boss:
            default:
                Debug.Log("Room Type not supported yet: " + this.type);
                break;
        }
    }


    private void SetCameraPriority()
    {
        if (type == Room.RoomType.Start)
        {
            GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().m_Priority = 11;
        }
    }
}