using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

public class DungeonGeneratorV2 : MonoBehaviour
{
    
    [SerializeField] private Vector2 worldSize = new Vector2(4, 4); //How big the level is in rooms (actual size is double that, 8x8, so that we can spawn the first one ~the middle)
    [SerializeField] private int numberOfRooms = 20; //Maximum number of rooms in the dungeon
    [SerializeField] private Vector2Int _roomSize = new Vector2Int(15, 15); //Size (in tiles) of the rooms
    [SerializeField] private RoomSpawn roomPrefab;
    [SerializeField] private PlayerController playerPrefab;
    
    private int _gridSizeX, _gridSizeY;
    private Room[,] _rooms; //(Empty) List of rooms
    private List<Vector2> _takenPositions = new List<Vector2>(); //Grid Positions where there's already a room
    
    public Vector2Int RoomSize => _roomSize;

    private void Start()
    {
        GenerateDungeon();
    }

    private void GenerateDungeon()
    {
        //Make sure the maximum number of rooms isn't higher than what the grid can fit
        if (numberOfRooms >= (worldSize.x * 2) * (worldSize.y * 2))
        {
            numberOfRooms = Mathf.RoundToInt((worldSize.x * 2) * (worldSize.y * 2));
        }

        _gridSizeX = Mathf.RoundToInt(worldSize.x);
        _gridSizeY = Mathf.RoundToInt(worldSize.y);
        CreateRooms();
        SetRoomDoors();
        CalculateDistanceFromStart();
        AssignRoomType();
        SpawnRooms();
        FindFirstObjectByType<EssentialsLoader>().PlacePlayer();
    }

    void CreateRooms()
    {
        //Setting the maximum number of rooms in the list
        _rooms = new Room[_gridSizeX * 2, _gridSizeY * 2];
        //Creating the starting room in the middle position (= gridSizeX, gridSizeY)
        _rooms[_gridSizeX, _gridSizeY] = new Room(Vector2.zero, Room.RoomType.Start, _roomSize);
        _takenPositions.Insert(0, Vector2.zero);
        Vector2 checkPos = Vector2.zero;

        //Numbers used for branching probability
        float randomCompare = 0.2f, randomCompareStart = 0.2f, randomCompareEnd = 0.01f;
        //Add rooms
        for (int i = 0; i < numberOfRooms - 1; i++)
        {
            float randomPerc = ((float)i) / (((float)numberOfRooms - 1));

            //The further the room is from the start, the less likely it is to branch out
            randomCompare = Mathf.Lerp(randomCompareStart, randomCompareEnd, randomPerc);
            checkPos = NewPosition();
            //Test the new position
            if (NumberOfNeighbors(checkPos, _takenPositions) > 1 && UnityEngine.Random.value > randomCompare)
            {
                int iterations = 0;
                do
                {
                    checkPos = SelectiveNewPosition();
                    iterations++;
                } while (NumberOfNeighbors(checkPos, _takenPositions) > 1 && iterations < 100);

                if (iterations >= 50)
                {
                    Debug.Log("error: could not create with fewer neighbors than : " +
                              NumberOfNeighbors(checkPos, _takenPositions));
                }
            }

            //finalize position, type of 0 means regular room. If more than 2 types, do it in a separate method once the layout of the map is complete
            //for example I could go through each room and check how many neighbours they have, if they have one that means they're at the end of a path and could be a boss or an important room
            _rooms[(int)checkPos.x + _gridSizeX, (int)checkPos.y + _gridSizeY] = new Room(checkPos, Room.RoomType.NA, _roomSize);

            // Add the new room to the list of taken positions
            _takenPositions.Insert(0, checkPos);
        }
    }

    Vector2 NewPosition()
    {
        int x = 0, y = 0;
        Vector2 checkingPos = Vector2.zero;
        do
        {
            int index = Mathf.RoundToInt(UnityEngine.Random.value * (_takenPositions.Count - 1));
            x = (int)_takenPositions[index].x;
            y = (int)_takenPositions[index].y;
            bool UpDown = (UnityEngine.Random.value < 0.5f);
            bool positive = (UnityEngine.Random.value < 0.5f);
            if (UpDown)
            {
                if (positive)
                {
                    y += 1;
                }
                else
                {
                    y -= 1;
                }
            }
            else
            {
                if (positive)
                {
                    x += 1;
                }
                else
                {
                    x -= 1;
                }
            }

            checkingPos = new Vector2(x, y);
        } while (_takenPositions.Contains(checkingPos) || x >= _gridSizeX || x < -_gridSizeX || y >= _gridSizeY ||
                 y < -_gridSizeY);

        return checkingPos;
    }

    //Returns a position that has only one neighbour. Could add a condition to only return rooms of normal type, to then change them and not have them in this pool
    Vector2 SelectiveNewPosition()
    {
        int index = 0, inc = 0;
        int x = 0, y = 0;
        Vector2 checkingPos = Vector2.zero;
        do
        {
            inc = 0;
            do
            {
                index = Mathf.RoundToInt(UnityEngine.Random.value * (_takenPositions.Count - 1));
                inc++;
            } while (NumberOfNeighbors(_takenPositions[index], _takenPositions) > 1 && inc < 100);

            x = (int)_takenPositions[index].x;
            y = (int)_takenPositions[index].y;
            bool UpDown = (UnityEngine.Random.value < 0.5f);
            bool positive = (UnityEngine.Random.value < 0.5f);
            if (UpDown)
            {
                if (positive)
                {
                    y += 1;
                }
                else
                {
                    y -= 1;
                }
            }
            else
            {
                if (positive)
                {
                    x += 1;
                }
                else
                {
                    x -= 1;
                }
            }

            checkingPos = new Vector2(x, y);
        } while (_takenPositions.Contains(checkingPos) || x >= _gridSizeX || x < -_gridSizeX || y >= _gridSizeY ||
                 y < -_gridSizeY);

        if (inc >= 100)
        {
            Debug.Log("Error: could not find position with only one neighbour");
        }

        return checkingPos;
    }

    int NumberOfNeighbors(Vector2 checkingPos, List<Vector2> usedPositions)
    {
        int ret = 0;
        if (usedPositions.Contains(checkingPos + Vector2.right))
        {
            ret++;
        }

        if (usedPositions.Contains(checkingPos + Vector2.left))
        {
            ret++;
        }

        if (usedPositions.Contains(checkingPos + Vector2.up))
        {
            ret++;
        }

        if (usedPositions.Contains(checkingPos + Vector2.down))
        {
            ret++;
        }

        return ret;
    }

    void SetRoomDoors()
    {
        for (int x = 0; x < ((_gridSizeX * 2)); x++)
        {
            for (int y = 0; y < ((_gridSizeY * 2)); y++)
            {
                if (_rooms[x, y] == null)
                {
                    continue;
                }

                Vector2 gridPosition = new Vector2(x, y);
                if (y - 1 < 0)
                {
                    _rooms[x, y].DoorBot = false;
                }
                else
                {
                    _rooms[x, y].DoorBot = (_rooms[x, y - 1] != null);
                }

                if (y + 1 >= _gridSizeY * 2)
                {
                    _rooms[x, y].DoorTop = false;
                }
                else
                {
                    _rooms[x, y].DoorTop = (_rooms[x, y + 1] != null);
                }

                if (x - 1 < 0)
                {
                    _rooms[x, y].DoorLeft = false;
                }
                else
                {
                    _rooms[x, y].DoorLeft = (_rooms[x - 1, y] != null);
                }

                if (x + 1 >= _gridSizeX * 2)
                {
                    _rooms[x, y].DoorRight = false;
                }
                else
                {
                    _rooms[x, y].DoorRight = (_rooms[x + 1, y] != null);
                }
            }
        }
    }

    void SpawnRooms()
    {
        foreach (Room room in _rooms)
        {
            if (room == null)
            {
                continue;
            }

            Vector2 drawPos = room.GridPos;
            drawPos.x *= room.Size.x + 2;
            drawPos.y *= room.Size.y + 2;
            
            RoomSpawn spawner = GameObject.Instantiate(roomPrefab, drawPos, Quaternion.identity, this.transform).GetComponent<RoomSpawn>();
            
            spawner.size = room.Size;
            spawner.type = room.Type;
            spawner.up = room.DoorTop;
            spawner.down = room.DoorBot;
            spawner.right = room.DoorRight;
            spawner.left = room.DoorLeft;
            spawner.spacesFromStart = room.SpacesFromStart;
            spawner.maxDifficulty = room.MaxDifficulty;
            spawner.completed = room.Completed;
            spawner.open = room.Open;
            spawner.monstersLeft = room.MonstersLeft;
        }
    }

    void CalculateDistanceFromStart()
    {
        Queue<Room> queue = new Queue<Room>();
        HashSet<Room> visited = new HashSet<Room>();

        // Start from the initial room
        Room startRoom = _rooms[_gridSizeX, _gridSizeY];
        startRoom.SpacesFromStart = 0;
        queue.Enqueue(startRoom);

        while (queue.Count > 0)
        {
            Room currentRoom = queue.Dequeue();
            visited.Add(currentRoom);

            // Check neighbors
            List<Room> neighbors = GetNeighbors(currentRoom);
            foreach (Room neighbor in neighbors)
            {
                if (!visited.Contains(neighbor))
                {
                    neighbor.SpacesFromStart = currentRoom.SpacesFromStart + 1;
                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    List<Room> GetNeighbors(Room room)
    {
        List<Room> neighbors = new List<Room>();

        int x = (int)room.GridPos.x + _gridSizeX;
        int y = (int)room.GridPos.y + _gridSizeY;

        if (x > 0 && _rooms[x - 1, y] != null)
            neighbors.Add(_rooms[x - 1, y]);
        if (x < _gridSizeX * 2 - 1 && _rooms[x + 1, y] != null)
            neighbors.Add(_rooms[x + 1, y]);
        if (y > 0 && _rooms[x, y - 1] != null)
            neighbors.Add(_rooms[x, y - 1]);
        if (y < _gridSizeY * 2 - 1 && _rooms[x, y + 1] != null)
            neighbors.Add(_rooms[x, y + 1]);

        return neighbors;
    }

    void AssignRoomType()
    {
        //Type logic goes here
        //Type for treasure rooms, shops if any, combat rooms, etc.
        
        List<Room> roomsByDistance = new List<Room>();
       foreach (Room room in _rooms)
       {
           if (room == null || room.Type == Room.RoomType.Start)
           {
               continue;
           }

           float treasureChance = Random.Range(0f, 1f);
           room.Type = treasureChance > 0.85f ? Room.RoomType.Treasure : Room.RoomType.Combat;
           roomsByDistance.Add(room);
       }

       //Make sure there's only one room with the highest distance from start
       roomsByDistance = roomsByDistance.OrderByDescending(r => r.SpacesFromStart).ToList();
       if (roomsByDistance.Count <= 1)
       {
           Debug.Log("One room or less");
           return;
       }
       if (roomsByDistance[0].SpacesFromStart == roomsByDistance[1].SpacesFromStart)
       {
           //If two end rooms have the same value, change one's value to +1
           Debug.Log("Two end rooms with same value: " + roomsByDistance[0].SpacesFromStart + " & " + roomsByDistance[1].SpacesFromStart);
           roomsByDistance[0].SpacesFromStart += 1;
           Debug.Log("New values: " + roomsByDistance[0].SpacesFromStart + " & " + roomsByDistance[1].SpacesFromStart);
       }
       else
       {
           Debug.Log("No two end rooms with same value: " + roomsByDistance[0].SpacesFromStart + " & " + roomsByDistance[1].SpacesFromStart);
       }
       //Assign the Boss Type to the furthest room from the start
       roomsByDistance[0].Type = Room.RoomType.Boss;
       roomsByDistance[1].Type = Room.RoomType.Shop;
    }
}