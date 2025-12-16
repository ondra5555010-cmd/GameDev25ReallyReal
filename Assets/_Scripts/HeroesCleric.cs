using System;
using UnityEngine;

public class HeroesCleric : BattleUnit
{
    void Start()
    {
        
    }

    private void Awake()
    {
        armorClass = 15;
        maxHitPoints = 19;
        maxMovementBudget = 4;
        damageDie = 8;
        attackBonus = 2;
        unitName = "Cleric";
        specialAbilityDescription = "Can cast 'Cure Wounds' which restores 1d8 HP to a friendly unit of your choice. (Press Shift)";
    }

    void Update()
    {
        
    }
    
    public override void ToggleSpecialMove()
    {
        BattleGrid.Instance.isSpecialMove = !BattleGrid.Instance.isSpecialMove;

        if (BattleGrid.Instance.isSpecialMove)
        {
            // Show reachable allied units for healing
            BattleGrid.Instance.RangedTargets(playerControlled);
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

        isActionReady = false;
        ShowFloatingText($"Cure Wounds", Color.cyan);

        // Roll 1d8 healing
        int healAmount = DiceRollManager.Instance.Roll(8);

        target.Heal(healAmount);

        // Update highlight of valid targets again
        BattleGrid.Instance.RangedTargets(playerControlled);
        RefreshUI();
    }
}
