using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class Entity
{
    [Header("Name")]
    public string name;
    public int level;

    [Header("Health")]
    public int currentHealth;
    public int maxHealth;

    [Header("Mana")]
    public int currentMana;
    public int maxMana;

    [Header("Stamina")]
    public int currentStamina;
    public int maxStamina;

    [Header("Status")]
    public int damage = 1;
    public int defense = 1;
    public int resistence = 1;
    public int strength = 1;
    public float speed = 60f;
    public int inteligence = 1;
    public int willPower = 1;
    public bool respawn = false;
    
    [Header("Combat")]
    public float attackDistance = 5f;
    public float attackTimer = 0.3f;
    public float attackDelay = 0.4f;
    public float attackRecharge = 0.3f;
    public float cooldown = 2;
    public bool inCombat = false;
    public GameObject target;
    public bool combatCoroutine = false;
    public bool dead = false;

}
