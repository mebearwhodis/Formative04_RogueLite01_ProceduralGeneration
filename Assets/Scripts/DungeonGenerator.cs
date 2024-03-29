using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object; 

public class DungeonGenerator : MonoBehaviour
{
    //How  big the level is in rooms (actual size is 8x8)
    Vector2 worldSize = new Vector2(4, 4);
    Room[,] rooms;
    List<Vector2> takenPositions = new List<Vector2>();
    int gridSizeX, gridSizeY, numberOfRooms = 20;
    public GameObject roomWhiteObj;
    public GameObject roomPrefab;
    public GameObject playerPrefab;
    public Vector2Int _roomSize = new Vector2Int(15, 15);

    private void Start()
    {
     GenerateDungeon();
     SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        // Instantiate the player prefab at the calculated position
        Instantiate(playerPrefab, new Vector3(_roomSize.x/2f, _roomSize.y/2f,0), Quaternion.identity);
    }

    public void GenerateDungeon()
    {
        //Make sure there aren't more rooms than the grid can fit
        if (numberOfRooms >= (worldSize.x * 2) * (worldSize.y * 2))
        {
            numberOfRooms = Mathf.RoundToInt((worldSize.x * 2) * (worldSize.y * 2));
        }
        gridSizeX = Mathf.RoundToInt(worldSize.x);
        gridSizeY = Mathf.RoundToInt(worldSize.y);
        CreateRooms();
        SetRoomDoors();
        //DrawMap();
        SpawnRooms();
    }

    void CreateRooms()
    {
        //Setting the rooms up
        rooms = new Room[gridSizeX * 2, gridSizeY * 2];
        //Creating the starting room (type 1)
        rooms[gridSizeX, gridSizeY] = new Room(Vector2.zero, 1, _roomSize);
        takenPositions.Insert(0, Vector2.zero);
        Vector2 checkPos = Vector2.zero;
        
        //magic numbers are bad, but this is just for testing
        float randomCompare = 0.2f, randomCompareStart = 0.2f, randomCompareEnd = 0.01f;
        //Add rooms
        for (int i = 0; i < numberOfRooms - 1; i++)
        {
            float randomPerc = ((float)i) / (((float)numberOfRooms - 1));
            
            //The further the room is from the start, the less likely it is to branch out
            randomCompare = Mathf.Lerp(randomCompareStart, randomCompareEnd, randomPerc);
            //grab a new position
            checkPos = NewPosition();
            //test the new position
            if (NumberOfNeighbors(checkPos, takenPositions) > 1 && UnityEngine.Random.value > randomCompare)
            {
                int iterations = 0;
                do
                {
                    checkPos = SelectiveNewPosition();
                    iterations++;
                } while (NumberOfNeighbors(checkPos, takenPositions) > 1 && iterations < 100);
                if (iterations >= 50)
                {
                    Debug.Log("error: could not create with fewer neighbors than : " + NumberOfNeighbors(checkPos, takenPositions));
                }
            }
            //finalize position, type of 0 means regular room. If more than 2 types, he suggests doing it in a separate method once the layout of the map is complete
            //for example I could go through each room and check how many neighbours they have, if they have one that means they're at the end of a path and could be a boss or an important room
            rooms[(int)checkPos.x + gridSizeX, (int)checkPos.y + gridSizeY] = new Room(checkPos, 0, _roomSize);
            
            // Check for square configurations and combine rooms
            //CombineRoomsIfSquare(checkPos);

            // Add the new room to the list of taken positions
            takenPositions.Insert(0, checkPos);
        }
    }
    Vector2 NewPosition()
    {
        int x = 0, y = 0;
        Vector2 checkingPos = Vector2.zero;
        do
        {
            int index = Mathf.RoundToInt(UnityEngine.Random.value * (takenPositions.Count - 1));
            x = (int)takenPositions[index].x;
            y = (int)takenPositions[index].y;
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
        } while (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY);
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
                index = Mathf.RoundToInt(UnityEngine.Random.value * (takenPositions.Count - 1));
                inc++;
            } while (NumberOfNeighbors(takenPositions[index], takenPositions) > 1 && inc < 100);
            x = (int)takenPositions[index].x;
            y = (int)takenPositions[index].y;
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
        } while (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY);
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
        for (int x = 0; x < ((gridSizeX * 2)); x++)
        {
            for (int y = 0; y < ((gridSizeY * 2)); y++)
            {
                if (rooms[x, y] == null)
                {
                    continue;
                }
                Vector2 gridPosition = new Vector2(x, y);
                if (y - 1 < 0)
                {
                    rooms[x, y].DoorBot = false;
                }
                else
                {
                    rooms[x, y].DoorBot = (rooms[x, y - 1] != null);
                }
                if (y + 1 >= gridSizeY * 2)
                {
                    rooms[x, y].DoorTop = false;
                }
                else
                {
                    rooms[x, y].DoorTop = (rooms[x, y + 1] != null);
                }
                if (x - 1 < 0)
                {
                    rooms[x, y].DoorLeft = false;
                }
                else
                {
                    rooms[x, y].DoorLeft = (rooms[x - 1, y] != null);
                }
                if (x + 1 >= gridSizeX * 2)
                {
                    rooms[x, y].DoorRight = false;
                }
                else
                {
                    rooms[x, y].DoorRight = (rooms[x + 1, y] != null);
                }
            }
        }
    }

    void DrawMap()
    {
        foreach (Room room in rooms)
        {
            if (room == null)
            {
                continue;
            }
            Vector2 drawPos = room.GridPos;
            drawPos.x *= room.Size.x +2;
            drawPos.y *= room.Size.y +2;
           MapSpriteSelector mapper = GameObject.Instantiate(roomWhiteObj, drawPos, Quaternion.identity, this.transform).GetComponent<MapSpriteSelector>();
           mapper.type = room.Type;
           mapper.up = room.DoorTop;
           mapper.down = room.DoorBot;
           mapper.right = room.DoorRight;
           mapper.left = room.DoorLeft;
        }
    }

    void SpawnRooms()
    {
        foreach (Room room in rooms)
        {
            if (room == null)
            {
                continue;
            }

            Vector2 drawPos = room.GridPos;
            drawPos.x *= room.Size.x +2;
            drawPos.y *= room.Size.y +2;
            RoomSpawn spawner = GameObject.Instantiate(roomPrefab, drawPos, Quaternion.identity, this.transform)
                .GetComponent<RoomSpawn>();
            spawner.size = room.Size;
            spawner.type = room.Type;
            spawner.up = room.DoorTop;
            spawner.down = room.DoorBot;
            spawner.right = room.DoorRight;
            spawner.left = room.DoorLeft;
        }
    }
    
    //Need rework
    // void CombineRoomsIfSquare(Vector2 newRoomPos)
    // {
    //     // Check if the newly added room creates a square configuration with its neighbors
    //     foreach (Vector2 direction in new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right })
    //     {
    //         Vector2 neighborPos = newRoomPos + direction;
    //         if (IsSquareConfiguration(newRoomPos, neighborPos))
    //         {
    //             CombineRooms(newRoomPos, neighborPos);
    //             break; // Exit loop after one square configuration is found and combined
    //         }
    //     }
    // }
    //
    // bool IsSquareConfiguration(Vector2 pos1, Vector2 pos2)
    // {
    //     // Check if pos1 and pos2 have two common neighbors
    //     // This indicates a potential square configuration
    //     int commonNeighbors = 0;
    //     foreach (Vector2 direction in new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right })
    //     {
    //         if (takenPositions.Contains(pos1 + direction) && takenPositions.Contains(pos2 + direction))
    //         {
    //             commonNeighbors++;
    //         }
    //     }
    //     return commonNeighbors == 2;
    // }
    //
    // void CombineRooms(Vector2 pos1, Vector2 pos2)
    // {
    //     // Remove the individual rooms and create a big room
    //     // Adjust room size and position accordingly
    //     Room room1 = rooms[(int)pos1.x + gridSizeX, (int)pos1.y + gridSizeY];
    //     Room room2 = rooms[(int)pos2.x + gridSizeX, (int)pos2.y + gridSizeY];
    //
    //     // Combine rooms and update room data
    //     Room bigRoom = new Room((room1.GridPos + room2.GridPos) / 2f, room1.Type, room1.Size + room2.Size);
    //     rooms[(int)pos1.x + gridSizeX, (int)pos1.y + gridSizeY] = bigRoom;
    //
    //     // Remove the other rooms
    //     rooms[(int)pos2.x + gridSizeX, (int)pos2.y + gridSizeY] = null;
    //     takenPositions.Remove(pos2);
    // }
}