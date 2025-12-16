using System;
using UnityEngine;

public class EnemiesSkelleton : BattleUnit
{
    void Start()
    {
        
    }

    private void Awake()
    {
        maxHitPoints = 13;
        armorClass = 12;
        damageDie = 6;
        attackBonus = 2;
        maxMovementBudget = 4;
        unitName = "Skeleton";
    }

    void Update()
    {
        
    }
    
}
