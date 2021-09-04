using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonController : MonoSingleton<DungeonController>
{
    private Dungeon currentDungeon;
    
    public TileSet TileSet;

    public int NoOfFloors;
    public Vector2Int RoomsPerFloor;
    public Vector2Int RoomsSize;

    public Floor CurrentFloor;

    private Vector2Int roomPosition;
    public Room CurrentRoom;

    public Tile GetTile(Vector2Int _position)
    {
        return CurrentRoom.Tiles[_position.x, _position.y];
    }

    public void CreateNewDungeon()
    {
        currentDungeon = new Dungeon();

        for (int i = 0; i < NoOfFloors; i++)
        {
            GameObject floorObj = new GameObject("Floor " + i);
            floorObj.transform.SetParent(transform);
            Floor floor = floorObj.AddComponent<Floor>();
            currentDungeon.floors.Add(floor);
            floor.Rooms = new Room[RoomsPerFloor.x, RoomsPerFloor.y];

            for (int x = 0; x < RoomsPerFloor.x; x++)
            {
                for (int y = 0; y < RoomsPerFloor.y; y++)
                {
                    GameObject roomObj = new GameObject("Room: " + x + ", " + y);
                    roomObj.transform.SetParent(floorObj.transform);
                    Room room = roomObj.AddComponent<Room>();
                    floor.Rooms[x, y] = room;

                    room.RoomPosition = new Vector2Int(x, y);
                    room.Tiles = new Tile[RoomsSize.x, RoomsSize.y];

                    for (int tilex = 0; tilex < RoomsSize.x; tilex++)
                    {
                        for (int tiley = 0; tiley < RoomsSize.y; tiley++)
                        {
                            GameObject tilePrototype = TileSet.GetTilePrototype(TilePrototype.eTitleID.Empty).PrefabObject;
                            Vector3 tilePosition = new Vector3(tilex, 0, tiley);
                            GameObject titleObj = GameObject.Instantiate(tilePrototype, tilePosition, Quaternion.identity);
                            titleObj.transform.SetParent(roomObj.transform);
                            room.Tiles[tilex, tiley] = titleObj.AddComponent<Tile>();
                            room.Tiles[tilex, tiley].Position = new Vector2Int(tilex, tiley);
                        }
                    }
                }
            }
        }
        
        //add doors in appropriate places
        foreach(Floor floor in currentDungeon.floors)
        {
            for (int x = 0; x < floor.Rooms.GetLength(0); x++)
            {
                for (int y = 0; y < floor.Rooms.GetLength(1); y++)
                {
                    Room room = floor.Rooms[x, y];
                    if (room != null)
                    {
                        Room neighbour = null;
                        
                        //UP TO DOWN
                        if (room.UpDoor == null && RoomHasNeighbour(floor, room, Vector2Int.up, out neighbour))
                        {
                            Vector2Int tilePosition = new Vector2Int(room.Size.x / 2, room.Size.y - 1);
                            room.UpDoor = AddDoor(floor, room, tilePosition);
                            if (neighbour.DownDoor == null)
                            {
                                Vector2Int neighBourTilePosition = new Vector2Int(room.Size.x / 2, 0);
                                neighbour.DownDoor = AddDoor(floor, neighbour, neighBourTilePosition);
                            }

                            room.UpDoor.TargetDoor = neighbour.DownDoor;
                            neighbour.DownDoor.TargetDoor = room.UpDoor;
                        }
                        
                        //DOWN TO UP
                        
                        if (room.DownDoor == null && RoomHasNeighbour(floor, room, Vector2Int.down, out neighbour))
                        {
                            Vector2Int tilePosition = new Vector2Int(room.Size.x / 2, 0);
                            room.DownDoor = AddDoor(floor, room, tilePosition);
                            if (neighbour.UpDoor == null)
                            {
                                Vector2Int neighBourTilePosition = new Vector2Int(room.Size.x / 2, room.Size.y - 1);
                                neighbour.UpDoor = AddDoor(floor, neighbour, neighBourTilePosition);
                            }

                            room.DownDoor.TargetDoor = neighbour.UpDoor;
                            neighbour.UpDoor.TargetDoor = room.DownDoor;
                        }   
                        
                        //LEFT TO RIGHT 
                        
                        if (room.LeftDoor == null && RoomHasNeighbour(floor, room, Vector2Int.left, out neighbour))
                        {
                            Vector2Int tilePosition = new Vector2Int(0, room.Size.y / 2);
                            room.LeftDoor = AddDoor(floor, room, tilePosition);
                            if (neighbour.RightDoor == null)
                            {
                                Vector2Int neighBourTilePosition = new Vector2Int(room.Size.x - 1,room.Size.y / 2);
                                neighbour.RightDoor = AddDoor(floor, neighbour, neighBourTilePosition);
                            }

                            room.LeftDoor.TargetDoor = neighbour.RightDoor;
                            neighbour.RightDoor.TargetDoor = room.LeftDoor;
                        }
                        
                        //RIGHT TO LEFT
                        
                        if (room.RightDoor == null && RoomHasNeighbour(floor, room, Vector2Int.right, out neighbour))
                        {
                            Vector2Int tilePosition = new Vector2Int(room.Size.x - 1, room.Size.y / 2);
                            room.RightDoor = AddDoor(floor, room, tilePosition);
                            if (neighbour.LeftDoor == null)
                            {
                                Vector2Int neighBourTilePosition = new Vector2Int(0, room.Size.y / 2);
                                neighbour.LeftDoor = AddDoor(floor, neighbour, neighBourTilePosition);
                            }

                            room.RightDoor.TargetDoor = neighbour.LeftDoor;
                            neighbour.LeftDoor.TargetDoor = room.RightDoor;
                        }
                        

                    }
                }
            }
        }

        for (int i = 0; i < currentDungeon.floors.Count - 1; i++)
        {
            bool placedFloorUp = false;
            do
            {
                if (TryGetRandomTile(currentDungeon.floors[i], out Tile tile, out Room room))
                {
                    if (i == 0)
                    {
                        roomPosition = room.RoomPosition;
                        GameController.Instance.Player.SetPosition(tile.Position);
                        room.gameObject.SetActive(true);
                        CurrentRoom = room;
                        CurrentFloor = currentDungeon.floors[0];
                    }
                    
                    TilePrototype doorPrototype = TileSet.GetTilePrototype(TilePrototype.eTitleID.Door);
                    GameObject doorPrefab = doorPrototype.PrefabObject;
                    GameObject newDoorObj = GameObject.Instantiate(doorPrefab, tile.transform.position, Quaternion.identity);
                    newDoorObj.transform.SetParent(tile.transform);
                    Door door = newDoorObj.GetComponent<Door>();
                    door.Passable = true;
                    tile.MapObjects.Add(door);
                    currentDungeon.floors[i].FloorUpDoor = door;
                    if (i > 0)
                    {
                        door.TargetDoor = currentDungeon.floors[i - 1].FloorDownDoor;
                    }
                    placedFloorUp = true;
                }
            } while (!placedFloorUp);
            
            bool placedFloorDown = false;
            do
            {
                if (TryGetRandomTile(currentDungeon.floors[i], out Tile tile, out Room room))
                {
                    placedFloorDown = true;
                    TilePrototype doorPrototype = TileSet.GetTilePrototype(TilePrototype.eTitleID.Door);
                    GameObject doorPrefab = doorPrototype.PrefabObject;
                    GameObject newDoorObj = GameObject.Instantiate(doorPrefab, tile.transform.position, Quaternion.identity);
                    newDoorObj.transform.SetParent(tile.transform);
                    Door door = newDoorObj.GetComponent<Door>();
                    door.Passable = true;
                    tile.MapObjects.Add(door);
                    currentDungeon.floors[i].FloorUpDoor = door;
                    door.TargetDoor = currentDungeon.floors[i + 1].FloorUpDoor;
                }
            } while (!placedFloorDown);
        }
    }
    
    

    bool TryGetRandomTile(Floor _floor, out Tile _tile, out Room _room)
    {
        Vector2Int pos = Vector2Int.zero;
        _tile = null;
        _room = _floor.Rooms[Random.Range(0, _floor.Rooms.GetLength(0)), Random.Range(0, _floor.Rooms.GetLength(1))];
        if (_room == null)
        {
            return false;
        }

        pos = new Vector2Int(Random.Range(0, _room.Size.x), Random.Range(0, _room.Size.y));
        _tile = _room.Tiles[pos.x, pos.y];
        if (!_tile.IsPassable())
        {
            return false;
        }

        return true;
    }

    bool RoomHasNeighbour(Floor _floor, Room _checkRoom, Vector2Int _direction, out Room _neighbour)
    {
        Vector2Int testPos = _checkRoom.RoomPosition + _direction;
        _neighbour = null;

        if (testPos.x < 0 || testPos.y < 0 || testPos.x >= RoomsPerFloor.x || testPos.y >= RoomsPerFloor.y)
        {
            return false;
        }

        if (_floor.Rooms[testPos.x, testPos.y] == null)
        {
            return false;
        }

        _neighbour = _floor.Rooms[testPos.x, testPos.y];

        return true;

    }

    Door AddDoor(Floor _floor, Room _room, Vector2Int _tilePosition)
    {
        TilePrototype doorPrototype = TileSet.GetTilePrototype(TilePrototype.eTitleID.Door);
        GameObject doorPrefab = doorPrototype.PrefabObject;
        Tile tile = _room.Tiles[_tilePosition.x, _tilePosition.y];
        GameObject newDoorObj = GameObject.Instantiate(doorPrefab, tile.transform.position, Quaternion.identity);
        Door door = newDoorObj.GetComponent<Door>();
        door.Floor = _floor;
        door.Room = _room;
        tile.MapObjects.Add(door);
        door.TilePosition = _tilePosition;
        newDoorObj.transform.SetParent(tile.gameObject.transform);
        door.Passable = true;
        return door;
    }
    

}
