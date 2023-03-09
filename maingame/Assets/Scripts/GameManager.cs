using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Int32 CalculateHealth(Entity entity)
    {
        Int32 result =
            (entity.resistence * 10) + (entity.level * 4) + 10;
        // Debug.LogFormat("reuslt od calculate health: {0}", result);

        return result;
    }

    public Int32 CalculateStamina(Entity entity)
    {
        Int32 result =
            (entity.resistence + entity.willPower) +
            (entity.level * 4) +
            10;
        // Debug.LogFormat("reuslt od calculate stamina: {0}", result);

        return result;
    }

    public Int32 CalculateMana(Entity entity)
    {
        Int32 result =
            (entity.resistence * 8) + (entity.level * 4) + 10;
        // Debug.LogFormat("reuslt od calculate mana: {0}", result);

        return result;
    }

    public Int32 CalculateDamage(Entity entity, int armorDamage)
    {
        System.Random rand = new System.Random();
        Int32 result =
            (entity.damage * 3) + (armorDamage * 2) + rand.Next(1, 20);
        // Debug.LogFormat("reuslt od calculate damage: {0}", result);
        
        return result;
    }
    
    public Int32 CalculateDefence(Entity entity, int armorDefence)
    {
        Int32 result = (entity.defense * 3) + (armorDefence * 2);
        // Debug.LogFormat("reuslt od calculate mana: {0}", result);

        return result;
    }
}
