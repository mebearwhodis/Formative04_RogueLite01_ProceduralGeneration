using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    
    public Vector2 GridPos;
    //Maybe make an enum for the type
    public int Type;
    public bool DoorTop, DoorBot, DoorLeft, DoorRight;
    public Vector2Int Size = new Vector2Int();
    //Also add a public int "value" to know how far from the start it is?
    public int SpacesFromStart;
    //Difficulty tied to how far it is?
    public int MaxDifficulty;
    
   

    public Room(Vector2 gridPos, int type, Vector2Int size)
    {
        GridPos = gridPos;
        Type = type;
        Size = size;
    }
}
