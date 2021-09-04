using System.Collections.Generic;
using UnityEngine;

public class Tile: MonoBehaviour
{
    public Vector2Int Position;
    public List<MapObject> MapObjects = new List<MapObject>();

    public bool IsPassable()
    {
        for (int i = 0; i < MapObjects.Count; i++)
        {
            if (!MapObjects[i].Passable)
            {
                return false;
            }
        }

        return true;
    }
}