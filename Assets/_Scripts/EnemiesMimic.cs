using System;
using UnityEngine;

public class EnemiesMimic : BattleUnit
{
    void Start()
    {
        
    }

    private void Awake()
    {
        armorClass = 30;
        maxHitPoints = 100;
        attackBonus = 5;
        maxMovementBudget = 3;
        unitName = "Mimic";
    }

    void Update()
    {
        
    }
    
}
