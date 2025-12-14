using System;
using UnityEngine;

public class HeroesWizard : BattleUnit
{
    void Start()
    {
        
    }

    private void Awake()
    {
        armorClass = 10;
        maxHitPoints = 12;
        maxMovementBudget = 4;
        damageDie = 6;
        attackBonus = 0;
        unitName = "Wizard";
        specialAbilityDescription = "Can cast 'Magic Missile' which causes ranged 1d4 damage. (Press Shift)";
    }
    
    public override void ToggleSpecialMove()
    {
        // Toggle the special move state
        BattleGrid.Instance.isSpecialMove = !BattleGrid.Instance.isSpecialMove;

        if (BattleGrid.Instance.isSpecialMove)
        {
            BattleGrid.Instance.RangedTargets(!playerControlled);
        }
        else
        {
            // Reset accessibility and show normal reachable tiles
            if (currentTile != null)
            {
                BattleGrid.Instance.ClearAllAccessible();
                BattleGrid.Instance.setupReachableTiles(this);
            }
        }

        Debug.Log($"{unitName} special move toggled: {BattleGrid.Instance.isSpecialMove}");
    }

    public override void EnactSpecialMove(BattleUnit target)
    {
        if (target == null || !isActionReady) return;

        // Cost the action
        isActionReady = false;

        // Roll 1d4 + 1 damage
        int damage = DiceRollManager.Instance.Roll(4) + 1;

        // Show floating text
        ShowFloatingText($"{damage} (Wizard Spell)", Color.cyan);

        // Apply damage
        target.takeDamage(damage);
        
        BattleGrid.Instance.RangedTargets(!playerControlled);
        
        RefreshUI();
    }

    void Update()
    {
        
    }
}
