using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monster Collection", menuName = "Create Monster Collection")]
public class MonsterCollection : ScriptableObject
{
    public List<MonsterPrototype> MonsterPrototypes = new List<MonsterPrototype>();

    public Monster GetMonster(MonsterPrototype.eMonsterID _type, int _level)
    {
        List<MonsterPrototype> possibleMonsters = new List<MonsterPrototype>();

        for (int i = 0; i < MonsterPrototypes.Count; i++)
        {
            if (MonsterPrototypes[i].ID == _type)
            {
                possibleMonsters.Add(MonsterPrototypes[i]);
            }
        }

        if (possibleMonsters.Count == 0)
        {
            Debug.LogError("No Monsters of Type" + _type);
        }

        MonsterPrototype prototype = possibleMonsters[Random.Range(0, possibleMonsters.Count)];

        GameObject newMonsterObject = GameObject.Instantiate(prototype.PrefabObject, Vector3.zero, Quaternion.identity);
        Monster monster = newMonsterObject.GetComponent<Monster>();
        
        monster.SetupMonster(prototype);

        return monster;
    }
}
