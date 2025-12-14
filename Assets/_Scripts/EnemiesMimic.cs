using System.Collections.Generic;
using UnityEngine;

public class EnemiesMimic : BattleUnit
{
    [Header("Boss Mechanics")]
    [SerializeField] private int acidCooldownTurns = 0; 
    private const int ACID_MAX_COOLDOWN = 3; 
    private const int ADHESIVE_DAMAGE = 2; 

    // Tato metoda nastaví staty, než BattleGrid zavolá Initialize()
    private void Awake()
    {
        unitName = "Elder Mimic (Boss)";
        
        // --- BOSS STATY ---
        armorClass = 20; 
        maxHitPoints = 80; 
        // Poznámka: currentHitPoints se nastaví v Initialize() na hodnotu maxHitPoints
        
        maxMovementBudget = 3; // Pomalý
        attackBonus = 8; 
        
        // Nastavení pro základní útok (používá base.Attack)
        // 2d10 (Pseudopod Slam)
        damageDice = 2; 
        damageDie = 10;
        
        specialAbilityDescription = $"Adhesive Skin: Melee attackers take {ADHESIVE_DAMAGE} dmg. Acid Eruption: 4d6 AOE (CD: {ACID_MAX_COOLDOWN}).";
    }

    // --- 1. ODPOČET COOLDOWNU (Voláno z TurnAndUnitsManager) ---
    public override void OnTurnStart()
    {
        base.OnTurnStart();

        // Pokud běží cooldown, snížíme ho
        if (acidCooldownTurns > 0)
        {
            acidCooldownTurns--;
            if (acidCooldownTurns == 0)
            {
                // Zobrazí text nad Mimicem
                ShowFloatingText("Acid Ready!", Color.magenta);
            }
        }
    }

    // --- 2. PASIVNÍ SCHOPNOST: ADHESIVE SKIN ---
    public override void takeDamage(int damage)
    {
        // 1. Nejprve aplikujeme poškození samotnému Mimicovi (a UI updates)
        base.takeDamage(damage);

        // 2. Logika pro lepivou kůži
        // Musíme zjistit, kdo nás praštil. 
        // Protože damage dostáváme v tahu hráče, útočník je pravděpodobně "SelectedUnit".
        
        if (UnitSelectionManager.Instance != null && UnitSelectionManager.Instance.selectedUnit != null)
        {
            BattleUnit attacker = UnitSelectionManager.Instance.selectedUnit;

            // Kontrola: Je to nepřítel (hráč) a stojí hned vedle nás?
            if (attacker.playerControlled && IsUnitAdjacent(attacker))
            {
                // Zobrazíme informaci o pasivce
                ShowFloatingText("Adhesive Skin!", Color.grey);

                // Udělíme poškození útočníkovi (Reflexivní poškození)
                // Použijeme jeho takeDamage, aby se mu odečetly životy a ukázalo číslo
                attacker.takeDamage(ADHESIVE_DAMAGE);
            }
        }
    }

    // --- 3. LOGIKA ÚTOKU (AI ROZHODOVÁNÍ) ---
    public override void Attack(BattleUnit target, bool isFree = false)
    {
        if (target == null || !isActionReady) return;

        // Spočítáme hráče v okolí
        int nearbyPlayers = CountAdjacentPlayers();

        // Podmínka pro speciální útok:
        // Cooldown je 0 A jsou tu alespoň 2 hráči (aby se AOE vyplatilo)
        if (acidCooldownTurns == 0 && nearbyPlayers >= 2)
        {
            PerformAcidEruption();
        }
        else
        {
            // Jinak provedeme základní útok
            // Využijeme logiku v rodičovské třídě BattleUnit (hodí d20 + attackBonus atd.)
            ShowFloatingText("Pseudopod Slam!", Color.red); // Jen info text navíc
            base.Attack(target, true); // true, protože akci odečítáme níže
        }

        // Odečtení akce (pokud to nebyl free attack)
        if (!isFree) 
            isActionReady = false;
        
        RefreshUI();
    }

    // --- SPECIÁLNÍ ÚTOK: ACID ERUPTION ---
    private void PerformAcidEruption()
    {
        ShowFloatingText("ACID ERUPTION!", new Color(0.7f, 1f, 0f)); // Jedovatě zelená
        
        // Reset cooldownu
        acidCooldownTurns = ACID_MAX_COOLDOWN; 

        // Získáme sousedy
        var neighbours = BattleGrid.Instance.GetNeighbouringUnits(this.currentTile);
        
        foreach (var victim in neighbours)
        {
            // Nezasáhneme sami sebe
            if (victim == this) continue;

            // Hod na poškození kyselinou: 4d6
            int acidDamage = DiceRollManager.Instance.Roll(6, 4); 
            
            // Informační text nad obětí (damage číslo se zobrazí v takeDamage automaticky červeně)
            victim.ShowFloatingText("Acid Burn", new Color(0.7f, 1f, 0f));
            
            // Aplikace poškození
            victim.takeDamage(acidDamage);
        }
    }

    // --- POMOCNÉ METODY ---
    
    private int CountAdjacentPlayers()
    {
        if (currentTile == null) return 0;
        var neighbours = BattleGrid.Instance.GetNeighbouringUnits(this.currentTile);
        int count = 0;
        foreach (var unit in neighbours)
        {
            if (unit.playerControlled) count++;
        }
        return count;
    }

    private bool IsUnitAdjacent(BattleUnit otherUnit)
    {
        if (otherUnit == null || otherUnit.currentTile == null || this.currentTile == null) return false;
        // Předpokládám, že BattleGrid má metodu GetDistance, která vrací Manhattan distance
        return BattleGrid.Instance.GetDistance(this.currentTile, otherUnit.currentTile) == 1;
    }
}