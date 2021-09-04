using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoSingleton<MonsterController>
{
    public MonsterCollection Monsters;
    public List<Monster> ActiveMonsters = new List<Monster>();

    private Queue<Monster> MonstersToMove = new Queue<Monster>();
    private Monster currentMonster = null;

    public Monster AddMonster(MonsterPrototype.eMonsterID _id, int _level)
    {
        Monster newMonster = Monsters.GetMonster(_id, _level);
        ActiveMonsters.Add(newMonster);
        return newMonster;
    }

    public void RemoveMonster(Monster _monster)
    {
        ActiveMonsters.Remove(_monster);
    }

    public void MoveMonster()
    {
        for (int i = 0; i < ActiveMonsters.Count; i++)
        {
            if (ActiveMonsters[i] != null && ActiveMonsters[i].isActiveAndEnabled)
            {
                MonstersToMove.Enqueue(ActiveMonsters[i]);
            }
        }

        if (MonstersToMove.Count == 0)
        {
            GameController.Instance.GoToState(GameController.eGameState.PlayerTurn);
        }
    }

    private void Update()
    {
        if (currentMonster != null && currentMonster.ActorState == Actor.eActorState.Idle)
        {
            currentMonster = null;
            if (MonstersToMove.Count == 0)
            {
                GameController.Instance.GoToState(GameController.eGameState.PlayerTurn);
            }
        }

        if (currentMonster == null && MonstersToMove.Count > 0)
        {
            currentMonster = MonstersToMove.Dequeue();
        }

        if (currentMonster != null && currentMonster.ActorState == Actor.eActorState.Idle)
        {
            currentMonster.DecideMove();
        }
    }
}
