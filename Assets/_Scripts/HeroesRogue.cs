using System;
using UnityEngine;

public class HeroesRogue : BattleUnit
{
    void Start()
    {
        
    }

    private void Awake()
    {
        armorClass = 13;
        maxHitPoints = 16;
        maxMovementBudget = 5;
        damageDie = 4;
        attackBonus = 5;
        unitName = "Rogue";
    }

    void Update()
    {
        
    }
}
