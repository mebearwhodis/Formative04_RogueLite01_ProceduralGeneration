using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Center : MonoBehaviour
{
    private void Start()
    {
        SetCenter();
    }

    private void SetCenter()
    {
        Vector2Int size = GetComponentInParent<RoomSpawn>().size;
        transform.position = (new Vector3((float)size.x / 2, (float)size.y / 2, 0));
    }
}