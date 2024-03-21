using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomSpawn : MonoBehaviour
{
    [SerializeField] private Tilemap _wallMap;
    [SerializeField] private Tilemap _floorMap;
    [SerializeField] private TileBase _wallTile;
    [SerializeField] private TileBase _floorTile;

    public Vector2Int size;
    
    public bool up, down, left, right;
    public int type; //0 = normal, 1 = start, 2 = end
    
    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (x == 0 || x == size.x - 1 || y == 0 || y == size.y - 1)
                {
                    _wallMap.SetTile(new Vector3Int(x, y, 0), _wallTile);
                }
                else
                {
                    _floorMap.SetTile(new Vector3Int(x, y, 0), _floorTile);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
