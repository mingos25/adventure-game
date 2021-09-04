using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monster Prototype", menuName = "Create Monster Prototype")]
public class MonsterPrototype : ScriptableObject
{
    public enum eMonsterID
    {
        Slime
    }

    public eMonsterID ID;

    public GameObject PrefabObject;

    public int Health;
    public int Damage;
    public bool Passable;
}
