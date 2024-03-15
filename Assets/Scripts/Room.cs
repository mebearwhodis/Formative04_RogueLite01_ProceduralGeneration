using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Vector2 GridPos;
    //Maybe make an enum for the type
    public int Type;
    //Also add a public int "value" to know how far from the start it is?
    public bool DoorTop, DoorBot, DoorLeft, DoorRight;

    public Room(Vector2 gridPos, int type)
    {
        GridPos = gridPos;
        Type = type;
    }
}
