using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class RoomSpawn : MonoBehaviour
{
    [SerializeField] private Tilemap _wallsMap;
    [SerializeField] private Tilemap _floorMap;
    [SerializeField] private Tilemap _doorsMap;
    [SerializeField] private Tilemap _roomBoundaries;
    [SerializeField] private TileBase _wallTile;
    [SerializeField] private TileBase _floorTile;
    [SerializeField] private TileBase _closedDoorTile;
    [SerializeField] private TileBase _openDoorTile;
    [SerializeField] private SpawnPoint _center;

    public Vector2Int size;

    public bool up, down, left, right;
    public int type; //0 = normal, 1 = start, 2 = end
    public int spacesFromStart;
    public bool completed;
    public bool open;

    // Start is called before the first frame update
    void Start()
    {
        DrawRoom();
        CreateDoors();
        SetCenter();
        FillRoom();
        SetCameraPriority();
    }

    private void Update()
    {
    }

    void DrawRoom()
    {
        // for (int x = -1; x <= size.x; x++)
        // {
        //     for (int y = -1; y <= size.y; y++)
        //     {
        //         if (x == -1 || x == size.x || y == -1 || y == size.y)
        //         {
        //             _wallsMap.SetTile(new Vector3Int(x, y, 0), _wallTile);
        //             _roomBoundaries.SetTile(new Vector3Int(x, y, 0), _wallTile);
        //         }
        //         else
        //         {
        //             _floorMap.SetTile(new Vector3Int(x, y, 0), _floorTile);
        //             _roomBoundaries.SetTile(new Vector3Int(x, y, 0), _wallTile);
        //         }
        //     }
        // }
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
    }

    void CreateDoors()
    {
        if (up)
        {
            _wallsMap.SetTile(new Vector3Int(size.x / 2, size.y, 0), null);
            _wallsMap.SetTile(new Vector3Int(size.x / 2, size.y + 1, 0), null);
            _doorsMap.SetTile(new Vector3Int(size.x / 2, size.y, 0), _openDoorTile);
            // _doorsMap.SetTile(new Vector3Int(size.x / 2, size.y + 1, 0), _openDoorTile);
            
            _doorsMap.SetTile(new Vector3Int(size.x / 2 -1, size.y, 0), _closedDoorTile);
            _doorsMap.SetTile(new Vector3Int(size.x / 2 +1, size.y, 0), _closedDoorTile);
            _doorsMap.SetTile(new Vector3Int(size.x / 2 -1, size.y + 1, 0), _closedDoorTile);
            _doorsMap.SetTile(new Vector3Int(size.x / 2 +1, size.y + 1, 0), _closedDoorTile);
        }
        if (down)
        {
            _wallsMap.SetTile(new Vector3Int(size.x / 2, -1, 0), null);
            _wallsMap.SetTile(new Vector3Int(size.x / 2, -2, 0), null);
            _doorsMap.SetTile(new Vector3Int(size.x / 2, -1, 0), _openDoorTile);
            // _doorsMap.SetTile(new Vector3Int(size.x / 2, -2, 0), _openDoorTile);
            
            _doorsMap.SetTile(new Vector3Int(size.x / 2 -1, -2, 0), _closedDoorTile);
            _doorsMap.SetTile(new Vector3Int(size.x / 2 +1, -2, 0), _closedDoorTile);
            _doorsMap.SetTile(new Vector3Int(size.x / 2 -1, -1, 0), _closedDoorTile);
            _doorsMap.SetTile(new Vector3Int(size.x / 2 +1, -1, 0), _closedDoorTile);
        }
        if (left)
        {
            _wallsMap.SetTile(new Vector3Int(-1, size.y / 2, 0), null);
            _wallsMap.SetTile(new Vector3Int(-2, size.y / 2, 0), null);
            _doorsMap.SetTile(new Vector3Int(-1, size.y / 2, 0), _openDoorTile);
            // _doorsMap.SetTile(new Vector3Int(-2, size.y / 2, 0), _openDoorTile);
            
            
            _doorsMap.SetTile(new Vector3Int(-2, size.y / 2 -1, 0), _closedDoorTile);
            _doorsMap.SetTile(new Vector3Int(-2, size.y / 2 +1, 0), _closedDoorTile);
            _doorsMap.SetTile(new Vector3Int(-1, size.y / 2 -1, 0), _closedDoorTile);
            _doorsMap.SetTile(new Vector3Int(-1, size.y / 2 +1, 0), _closedDoorTile);
        }
        if (right)
        {
            _wallsMap.SetTile(new Vector3Int(size.x, size.y / 2, 0), null);
            _wallsMap.SetTile(new Vector3Int(size.x + 1, size.y / 2, 0), null);
            _doorsMap.SetTile(new Vector3Int(size.x, size.y / 2, 0), _openDoorTile);
            // _doorsMap.SetTile(new Vector3Int(size.x + 1, size.y / 2, 0), _openDoorTile);
            
            _doorsMap.SetTile(new Vector3Int(size.x, size.y / 2 -1, 0), _closedDoorTile);
            _doorsMap.SetTile(new Vector3Int(size.x, size.y / 2 +1, 0), _closedDoorTile);
            _doorsMap.SetTile(new Vector3Int(size.x + 1, size.y / 2 -1, 0), _closedDoorTile);
            _doorsMap.SetTile(new Vector3Int(size.x + 1, size.y / 2 +1, 0), _closedDoorTile);
            
        }
    }

    public void ToggleDoors(bool state)
    {
        _doorsMap.GetComponent<TilemapCollider2D>().enabled = state;
        Debug.Log("Toggled doors: " + state);
        if (_doorsMap.GetComponent<TilemapCollider2D>().enabled == true)
        {
            //_doorsMap.GetComponent<TilemapRenderer>().sortingOrder = 3;
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
            //_doorsMap.GetComponent<TilemapRenderer>().sortingOrder = 0;
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

    void SetCenter()
    {
        _center.transform.position = new Vector3((float)size.x / 2, (float)size.y / 2, 0);
    }
    
    void FillRoom()
    {
        switch (type)
        {
            case 1:
                _center.gameObject.SetActive(true);
                completed = true;
                open = true;
                _doorsMap.GetComponent<TilemapCollider2D>().enabled = false;
                break;
            default:
                Debug.Log("Room Type not supported yet");
                Debug.Log("Spaces from the start : " + spacesFromStart);
                break;
        }
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