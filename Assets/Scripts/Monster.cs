using System.Collections.Generic;
using UnityEngine;

public class Monster : Actor
{
    private Reward killReward;

    public int Damage;

    public void SetupMonster(MonsterPrototype _prototype)
    {
        Damage = _prototype.Damage;
        currentHealth = _prototype.Health;
        maxHealth = _prototype.Health;
        Passable = _prototype.Passable;
    }

    public virtual void DecideMove()
    {
        Vector2Int playerPosition = GameController.Instance.Player.TilePosition;

        List<Vector2Int> possibleDirections = new List<Vector2Int>();

        if (TilePosition.x != 0)
        {
            possibleDirections.Add(Vector2Int.left);
        }

        if (TilePosition.x != DungeonController.Instance.CurrentRoom.Tiles.GetLength(0) - 1)
        {
            possibleDirections.Add(Vector2Int.right);
        }

        if (TilePosition.y != 0)
        {
            possibleDirections.Add(Vector2Int.down);
        }

        if (TilePosition.y != DungeonController.Instance.CurrentRoom.Tiles.GetLength(1) - 1)
        {
            possibleDirections.Add(Vector2Int.up);
        }

        Vector2Int shortestDirection = possibleDirections[0];
        for (int i = 0; i < possibleDirections.Count; i++)
        {
            Vector2Int difference = TilePosition + possibleDirections[i] - playerPosition;
            Vector2Int shortestDifference = TilePosition + shortestDirection - playerPosition;
            if (difference.magnitude <= shortestDifference.magnitude)
            {
                shortestDirection = possibleDirections[i];   
            }
        }

        Vector2Int movePosition = TilePosition + shortestDirection;
        bool attacking = false;
        for (int i = 0; i < DungeonController.Instance.GetTile(movePosition).MapObjects.Count; i++)
        {
            if (DungeonController.Instance.GetTile(movePosition).MapObjects[i].GetType() == typeof(Player))
            {
                attacking = true;
                base.BeginAttack(shortestDirection);
                break;
            }
        }

        if (!attacking && DungeonController.Instance.GetTile(movePosition).IsPassable())
        {
            DungeonController.Instance.GetTile(movePosition).MapObjects.Remove(this);
            base.BeginMove(shortestDirection);
        }
    }
}
