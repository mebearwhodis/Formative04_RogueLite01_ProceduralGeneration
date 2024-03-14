using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class IslandGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap _generatedMap;
    [SerializeField] private Tilemap _landMap;
    [SerializeField] private Tilemap _seaMap;

    [SerializeField] private TileBase _water;
    [SerializeField] private TileBase _land;

    [SerializeField] [Range(0, 100)] private int _fillPercent;
    [SerializeField] Vector2Int _size;

    [SerializeField] private string seed;
    [SerializeField] private bool useRandomSeed;

    private void Start()
    {
        GenerateMap();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GenerateMap();
        }
    }

    public void GenerateMap()
    {
        _generatedMap.ClearAllTiles();
        RandomFillmap();

        for (int i = 0; i < 5; i++)
        {
            SmoothMap();
        }

        ProcessMap();
        SeparateMaps();
    }

    void ProcessMap()
    {
        List<List<Coord>> wallRegions = GetRegions(_water);

        //Delete small 'island' of walls in rooms,
        //Any wall region with less than 50 tiles is removed, make it serializable?
        int wallThresholdSize = 50;

        foreach (List<Coord> wallRegion in wallRegions)
        {
            if (wallRegion.Count < wallThresholdSize)
            {
                foreach (Coord tile in wallRegion)
                {
                    _generatedMap.SetTile(new Vector3Int(tile.tileX, tile.tileY, 0), _land);
                }
            }
        }

        List<List<Coord>> roomRegions = GetRegions(_land);

        //Any room region with less than 50 tiles is removed, make it serializable?
        int roomThresholdSize = 50;
        List<Room> survivingRooms = new List<Room>();

        foreach (List<Coord> roomRegion in roomRegions)
        {
            if (roomRegion.Count < roomThresholdSize)
            {
                foreach (Coord tile in roomRegion)
                {
                    _generatedMap.SetTile(new Vector3Int(tile.tileX, tile.tileY, 0), _water);
                }
            }
            else
            {
                survivingRooms.Add(new Room(roomRegion, _generatedMap, _water));
            }
        }

        survivingRooms.Sort();
        survivingRooms[0].isMainRoom = true;
        survivingRooms[0].isAccessibleFromMainRoom = true;

        ConnectClosestRooms(survivingRooms);
    }

    private void SeparateMaps()
    {
        _landMap.ClearAllTiles();
        _seaMap.ClearAllTiles();

        for (int x = 0; x < _size.x; x++)
        {
            for (int y = 0; y < _size.y; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                TileBase tile = _generatedMap.GetTile(tilePosition);

                if (tile == _land)
                {
                    _landMap.SetTile(tilePosition, _land);
                }
                else if (tile == _water)
                {
                    _seaMap.SetTile(tilePosition, _water);
                }
            }
        }

        _generatedMap.ClearAllTiles();
    }


    void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false)
    {
        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();

        if (forceAccessibilityFromMainRoom)
        {
            foreach (Room room in allRooms)
            {
                if (room.isAccessibleFromMainRoom)
                {
                    roomListB.Add(room);
                }
                else
                {
                    roomListA.Add(room);
                }
            }
        }
        else
        {
            roomListA = allRooms;
            roomListB = allRooms;
        }

        int bestDistance = 0;
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool possibleConnectionFound = false;

        foreach (Room roomA in roomListA)
        {
            if (!forceAccessibilityFromMainRoom)
            {
                possibleConnectionFound = false;
                //Skip to the next room if this one already has a connection
                if (roomA.connectedRooms.Count > 0)
                {
                    continue;
                }
            }

            foreach (Room roomB in roomListB)
            {
                if (roomA == roomB || roomA.IsConnected(roomB))
                {
                    continue;
                }

                for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
                {
                    for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
                    {
                        Coord tileA = roomA.edgeTiles[tileIndexA];
                        Coord tileB = roomB.edgeTiles[tileIndexB];
                        int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) +
                                                         Mathf.Pow(tileA.tileY - tileB.tileY, 2));
                        if (distanceBetweenRooms < bestDistance || !possibleConnectionFound)
                        {
                            bestDistance = distanceBetweenRooms;
                            possibleConnectionFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }

            if (possibleConnectionFound && !forceAccessibilityFromMainRoom)
            {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }

        if (possibleConnectionFound && forceAccessibilityFromMainRoom)
        {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(allRooms, true);
        }

        //First iteration of the method connects rooms closest to each other but stops once every room is connected,
        //it does not check if they're all connected to the main room. Second iteration will check every room that is
        //not connected to the main room, and pick the closest one of each group to connect to the closest
        //that is connected to the main room. (Yeah it's a bit complicated)
        if (!forceAccessibilityFromMainRoom)
        {
            ConnectClosestRooms(allRooms, true);
        }
    }

    void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB)
    {
        Room.ConnectRooms(roomA, roomB);

        List<Coord> line = GetLine(tileA, tileB);
        foreach (Coord c in line)
        {
            DrawCircle(c, 2);
        }
    }

    void DrawCircle(Coord c, int r)
    {
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                //If the coord is inside the 'circle'
                if (x * x + y * y <= r * r)
                {
                    //Get the coordinates
                    int drawX = c.tileX + x;
                    int drawY = c.tileY + y;
                    //Check if it's inside the map
                    if (IsInMapRange(drawX, drawY))
                    {
                        //Set the tile to be empty/floor
                        _generatedMap.SetTile(new Vector3Int(drawX, drawY, 0), _land);
                    }
                }
            }
        }
    }

    //Gets a line of tiles between two points
    List<Coord> GetLine(Coord from, Coord to)
    {
        List<Coord> line = new List<Coord>();

        int x = from.tileX;
        int y = from.tileY;

        int dx = to.tileX - from.tileX;
        int dy = to.tileY - from.tileY;

        bool inverted = false;
        int step = Math.Sign(dx);
        int gradientStep = Math.Sign(dy);

        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if (longest < shortest)
        {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            step = Math.Sign(dy);
            gradientStep = Math.Sign(dx);
        }

        int gradientAccumulation = longest / 2;
        for (int i = 0; i < longest; i++)
        {
            line.Add(new Coord(x, y));

            if (inverted)
            {
                y += step;
            }
            else
            {
                x += step;
            }

            gradientAccumulation += shortest;
            if (gradientAccumulation >= longest)
            {
                if (inverted)
                {
                    x += gradientStep;
                }
                else
                {
                    y += gradientStep;
                }

                gradientAccumulation -= longest;
            }
        }

        return line;
    }

    Vector3 CoordToWorldPoint(Coord tile)
    {
        return new Vector3(-_size.x / 2 + .5f + tile.tileX, -_size.y / 2 + .5f + tile.tileY);
    }

    List<List<Coord>> GetRegions(TileBase tileType)
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[_size.x, _size.y];

        for (int x = 0; x < _size.x; x++)
        {
            for (int y = 0; y < _size.y; y++)
            {
                if (mapFlags[x, y] == 0 && (_generatedMap.GetTile(new Vector3Int(x, y, 0)) == tileType))
                {
                    List<Coord> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);

                    foreach (Coord tile in newRegion)
                    {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }

        return regions;
    }

    List<Coord> GetRegionTiles(int startX, int startY)
    {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[_size.x, _size.y];
        TileBase tileType = _generatedMap.GetTile(new Vector3Int(startX, startY, 0));

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (IsInMapRange(x, y) && (y == tile.tileY || x == tile.tileX))
                    {
                        if (mapFlags[x, y] == 0 && _generatedMap.GetTile(new Vector3Int(x, y, 0)) == tileType)
                        {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }

        return tiles;
    }

    bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < _size.x && y >= 0 && y < _size.y;
    }

    private void RandomFillmap()
    {
        if (useRandomSeed)
        {
            seed = Time.time.ToString();
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < _size.x; x++)
        {
            for (int y = 0; y < _size.y; y++)
            {
                if (x == 0 || x == _size.x - 1 || y == 0 || y == _size.y - 1)
                {
                    _generatedMap.SetTile(new Vector3Int(x, y, 0), _water);
                }
                else
                {
                    _generatedMap.SetTile(new Vector3Int(x, y, 0),
                        (pseudoRandom.Next(0, 100) < _fillPercent) ? _water : _land);
                }
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < _size.x; x++)
        {
            for (int y = 0; y < _size.y; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                //Could make the conditions serializable
                if (neighbourWallTiles > 4)
                {
                    _generatedMap.SetTile(new Vector3Int(x, y, 0), _water);
                }
                else if (neighbourWallTiles < 4)
                {
                    _generatedMap.SetTile(new Vector3Int(x, y, 0), _land);
                }
            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (IsInMapRange(neighbourX, neighbourY))
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += (_generatedMap.GetTile(new Vector3Int(neighbourX, neighbourY, 0)) == _water)
                            ? 1
                            : 0;
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

    struct Coord
    {
        public int tileX;
        public int tileY;

        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
    }

    //Connect rooms
    class Room : IComparable<Room>
    {
        public List<Coord> tiles;
        public List<Coord> edgeTiles;
        public List<Room> connectedRooms;
        public int roomSize;
        public bool isAccessibleFromMainRoom;
        public bool isMainRoom;

        public Room()
        {
        }

        public Room(List<Coord> roomTiles, Tilemap map, TileBase wall)
        {
            tiles = roomTiles;
            roomSize = tiles.Count;
            connectedRooms = new List<Room>();

            edgeTiles = new List<Coord>();

            //Checking each tile, if any of its neighbours is a wall, it's an edge tile
            foreach (Coord tile in tiles)
            {
                for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                {
                    for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                    {
                        if (x == tile.tileX || y == tile.tileY)
                        {
                            if (map.GetTile(new Vector3Int(x, y, 0)) == wall)
                            {
                                edgeTiles.Add(tile);
                            }
                        }
                    }
                }
            }
        }

        //When we connect 2 rooms, if one of them is accessible from the main room,
        //we set the other one and all the ones it's connected to to be accessible too
        public void SetAccessibleFromMainRoom()
        {
            if (!isAccessibleFromMainRoom)
            {
                isAccessibleFromMainRoom = true;
                foreach (Room connectedRoom in connectedRooms)
                {
                    connectedRoom.SetAccessibleFromMainRoom();
                }
            }
        }

        //Connects two rooms
        public static void ConnectRooms(Room roomA, Room roomB)
        {
            if (roomA.isAccessibleFromMainRoom)
            {
                roomB.SetAccessibleFromMainRoom();
            }
            else if (roomB.isAccessibleFromMainRoom)
            {
                roomA.SetAccessibleFromMainRoom();
            }

            roomA.connectedRooms.Add(roomB);
            roomB.connectedRooms.Add(roomA);
        }

        //Checks if two rooms are connected
        public bool IsConnected(Room otherRoom)
        {
            return connectedRooms.Contains(otherRoom);
        }

        public int CompareTo(Room otherRoom)
        {
            return otherRoom.roomSize.CompareTo(roomSize);
        }
    }
}