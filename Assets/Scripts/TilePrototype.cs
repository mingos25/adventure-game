using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tile Prototype", menuName = "Create Tile Prototype")]
public class TilePrototype : ScriptableObject
{
    public enum eTitleID
    {
        Empty,
        Door,
        FloorUp,
        FloorDown
    }

    public eTitleID TileType;
    public GameObject PrefabObject;
}
