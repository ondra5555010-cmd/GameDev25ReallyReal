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
        specialAbilityDescription = "Causes 2d6 extra damage against enemies adjacent to friendly units.";
    }
    
    public override void Attack(BattleUnit target, bool isFree = false)
    {
        if (target == null) return;

        if (!isFree)
            isActionReady = false;

        int d20 = DiceRollManager.Instance.Roll(20);
        int attackRoll = d20 + attackBonus;
        bool hit = attackRoll >= target.armorClass;

        Color rollColor = hit ? Color.green : new Color(1f, 0.5f, 0f);
        ShowFloatingText($"Attack {attackRoll} vs AC {target.armorClass}", rollColor);

        if (!hit) return;

        int damage = DiceRollManager.Instance.Roll(damageDie, damageDice);
        
        ShowFloatingText($"{damageDice}d{damageDie}", Color.yellow);

        // --- Sneak Attack check ---
        bool sneakAttack = false;

        var neighbours = BattleGrid.Instance.GetNeighbouringUnits(target.currentTile);
        foreach (var unit in neighbours)
        {
            if (unit == this) continue;
            if (unit.playerControlled == this.playerControlled)
            {
                sneakAttack = true;
                break;
            }
        }

        if (sneakAttack)
        {
            int sneakDamage = DiceRollManager.Instance.Roll(6, 2); // 2d6
            damage += sneakDamage;
            ShowFloatingText("Sneak Attack 2d6", Color.cyan);
        }
        
        target.takeDamage(damage);
        RefreshUI();
    }

    void Update()
    {
        
    }
}
