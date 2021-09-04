using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TileSet", menuName = "Create TileSet")]
public class TileSet : ScriptableObject
{
    public List<TilePrototype> TilePrototypes = new List<TilePrototype>();

    public TilePrototype GetTilePrototype(TilePrototype.eTitleID _type)
    {
        List<TilePrototype> possibleTiles = new List<TilePrototype>();

        for (int i = 0; i < TilePrototypes.Count; i++)
        {
            if (TilePrototypes[i].TileType == _type)
            {
                possibleTiles.Add(TilePrototypes[i]);
            }
        }

        if (possibleTiles.Count == 0)
        {
            Debug.LogError("No Tile For Type: " + _type);
        }

        return possibleTiles[Random.Range(0, possibleTiles.Count)];
    }
}
