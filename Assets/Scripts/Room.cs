using UnityEngine;

public class Room : MonoBehaviour
{
    public Tile[,] Tiles;
    public Vector2Int RoomPosition;

    public Vector2Int Size
    {
        get { return new Vector2Int(Tiles.GetLength(0), Tiles.GetLength(1)); }
    }

    public Door UpDoor, DownDoor, LeftDoor, RightDoor;
}
