using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    
    public enum RoomType
    {
        NA,
        Start,
        Treasure,
        Shop,
        Combat,
        Boss
    }
    
    public Vector2 GridPos;
    public RoomType Type;
    public bool DoorTop, DoorBot, DoorLeft, DoorRight;
    public Vector2Int Size = new Vector2Int();
    public int SpacesFromStart;
    public int MaxDifficulty;
    public int MonstersLeft;

    public bool Completed = false;
    public bool Open = true;
    
    public Room(Vector2 gridPos, RoomType type, Vector2Int size)
    {
        GridPos = gridPos;
        Type = type;
        Size = size;
    }
}

