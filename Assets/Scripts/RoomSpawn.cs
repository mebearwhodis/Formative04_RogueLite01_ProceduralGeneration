using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomSpawn : MonoBehaviour
{
    [SerializeField] private Tilemap _wallMap;
    [SerializeField] private Tilemap _floorMap;
    [SerializeField] private Tilemap _roomBoundaries;
    [SerializeField] private TileBase _wallTile;
    [SerializeField] private TileBase _floorTile;

    public Vector2Int size;

    public bool up, down, left, right;
    public int type; //0 = normal, 1 = start, 2 = end
    public int spacesFromStart;

    // Start is called before the first frame update
    void Start()
    {
        DrawRoom();
        CreateDoors();
        FillRoom();
        SetCameraPriority();
    }

    void DrawRoom()
    {
        for (int x = -1; x <= size.x; x++)
        {
            for (int y = -1; y <= size.y; y++)
            {
                if (x == -1 || x == size.x || y == -1 || y == size.y)
                {
                    _wallMap.SetTile(new Vector3Int(x, y, 0), _wallTile);
                    _roomBoundaries.SetTile(new Vector3Int(x, y, 0), _wallTile);
                }
                else
                {
                    _floorMap.SetTile(new Vector3Int(x, y, 0), _floorTile);
                    _roomBoundaries.SetTile(new Vector3Int(x, y, 0), _wallTile);
                }
            }
        }
        _roomBoundaries.color = Color.clear;
    }

    void CreateDoors()
    {
        if (up)
        {
            _wallMap.SetTile(new Vector3Int(size.x / 2, size.y, 0), null);
            _floorMap.SetTile(new Vector3Int(size.x / 2, size.y, 0), _floorTile);
        }
        if (down)
        {
            _wallMap.SetTile(new Vector3Int(size.x / 2, -1, 0), null);
            _floorMap.SetTile(new Vector3Int(size.x / 2, -1, 0), _floorTile);
        }
        if (left)
        {
            _wallMap.SetTile(new Vector3Int(-1, size.y / 2, 0), null);
            _floorMap.SetTile(new Vector3Int(-1, size.y / 2, 0), _floorTile);
        }
        if (right)
        {
            _wallMap.SetTile(new Vector3Int(size.x, size.y / 2, 0), null);
            _floorMap.SetTile(new Vector3Int(size.x, size.y / 2, 0), _floorTile);
        }
    }

    void FillRoom()
    {
        //Depending on type, do different things when the player enters the room
    }
    
    void SetCameraPriority()
    {
        if (type == 1)
        {
            GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().m_Priority = 11;
        }
    }
}