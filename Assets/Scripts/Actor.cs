using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MapObject
{
    protected Vector2Int targetPosition;

    protected int maxHealth;
    protected int currentHealth;

    public float MoveSpeed = 5.0f;
    public int InitialHealth;

    public enum eActorState
    {
        Idle,
        Moving,
        Attacking,
        Dead
    }

    public eActorState ActorState;

    private Actor attackTarget;
    private float attackDuration = 0.5f;
    private float attackStartTime;

    public void SetPosition(Vector2Int _position)
    {
        TilePosition = _position;
        transform.position = new Vector3(TilePosition.x, 0, TilePosition.y);
    }

    public virtual bool CanMoveToTile(Vector2Int _position)
    {
        if (ActorState != eActorState.Idle || _position.x < 0 || _position.y < 0 ||
            _position.x >= DungeonController.Instance.CurrentRoom.Size.x || _position.y >= DungeonController.Instance.CurrentRoom.Size.y)
        {
            return false;
        }

        return true;
    }

    public virtual void BeginMove(Vector2 _direction)
    {
        Vector2Int direction = new Vector2Int((int) _direction.x, (int) _direction.y);
        Vector2Int position = TilePosition + direction;

        if (!CanMoveToTile(position))
        {
            return;
        }

        Tile tile = DungeonController.Instance.GetTile(position);
        if (tile != null)
        {
            if (tile.IsPassable())
            {
                for (int i = 0; i < tile.MapObjects.Count; i++)
                {
                    if (tile.MapObjects[i].GetType() == typeof(Door))
                    {
                        ActorState = eActorState.Moving;
                        targetPosition = position;
                    }
                }

                if (ActorState == eActorState.Idle)
                {
                    ActorState = eActorState.Moving;
                    targetPosition = position;
                }
            }
            else
            {
                //handle what is blocking movement
                for (int i = 0; i < tile.MapObjects.Count; i++)
                {
                    if (tile.MapObjects[i].GetType() == typeof(Monster))
                    {
                        attackStartTime = Time.time;
                        ActorState = eActorState.Attacking;
                        attackTarget = tile.MapObjects[i] as Monster;
                    }
                }
            }
        }
    }

    public virtual void BeginAttack(Vector2 _direction)
    {
        Vector2Int direction = new Vector2Int((int) _direction.x, (int) _direction.y);
        Vector2Int position = TilePosition + direction;

        if (!CanMoveToTile(position))
        {
            return;
        }

        bool hasPlayer = false;
        for (int i = 0; i < DungeonController.Instance.GetTile(position).MapObjects.Count; i++)
        {
            if (DungeonController.Instance.GetTile(position).MapObjects[i].GetType() == typeof(Player))
            {
                hasPlayer = true;
            }
        }

        if (!hasPlayer)
        {
            return;
        }

        GameController.Instance.Player.TakeDamage(GetAttackDamage());
    }

    protected virtual void Update()
    {
        switch (ActorState)
        {
            case eActorState.Moving:
                Vector3 targetPos = new Vector3(targetPosition.x, 0, targetPosition.y);
                if (Vector3.Distance(transform.position, targetPos) > float.Epsilon)   
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * MoveSpeed);
                }
                else
                {
                    EnterTile(targetPosition);
                    EndTurn();
                }
                break;
            
            case eActorState.Attacking:
                Vector3 attackPos = new Vector3(attackTarget.TilePosition.x, 0, attackTarget.TilePosition.y);
                float t = (Time.time - attackStartTime) / attackDuration;
                Vector3 attackDir = attackPos - transform.position;
                transform.position = DungeonController.Instance.CurrentRoom.Tiles[TilePosition.x, TilePosition.y].gameObject.transform.position +
                                     attackDir * Mathf.PingPong(t, 0.5f);
                if (t > 1f)
                {
                    int attackDamage = GetAttackDamage();
                    attackTarget.TakeDamage(attackDamage);
                    EndTurn();
                }
                break;
        }
    }

    protected virtual void EndTurn()
    {
        ActorState = eActorState.Idle;
    }

    private int GetAttackDamage()
    {
        return 5;
    }

    public void TakeDamage(int _amount)
    {
        int totalDamage = _amount;

        if (_amount >= currentHealth)
        {
            totalDamage = currentHealth;
        }

        currentHealth -= totalDamage;

        if (currentHealth <= 0)
        {
            OnKill();
        }
    }

    public virtual void OnKill()
    {
        DungeonController.Instance.GetTile(TilePosition).MapObjects.Remove(this);
        ActorState = eActorState.Dead;
        Destroy(gameObject);
    }

    protected virtual void EnterTile(Vector2Int _tilePosition)
    {
        DungeonController.Instance.GetTile(_tilePosition).MapObjects.Remove(this);
        TilePosition = targetPosition;
        DungeonController.Instance.GetTile(_tilePosition).MapObjects.Add(this);
    }
}
