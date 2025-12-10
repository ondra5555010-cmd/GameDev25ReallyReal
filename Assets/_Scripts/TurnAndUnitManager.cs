using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnAndUnitManager : MonoBehaviour
{
    public static TurnAndUnitManager Instance;
    public bool isPlayerTurn = true;
    
    public List<BattleUnit> PlayerUnits { get; private set; }
    public List<BattleUnit> EnemyUnits  { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        PlayerUnits = new List<BattleUnit>();
        EnemyUnits  = new List<BattleUnit>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isPlayerTurn)
        {
            EndTurn();
        }
    }

    private void toggleIsPlayerTurn()
    {
        isPlayerTurn = !isPlayerTurn;
    }

    public void EndTurn()
    {
        UnitSelectionManager.Instance.DeselectUnit();
        toggleIsPlayerTurn();

        if (isPlayerTurn)
        {
            refreshFactionMovementBudget(PlayerUnits);
        }
        else
        {
            refreshFactionMovementBudget(EnemyUnits);
            StartCoroutine(RunAITurn());
        }
    }
    
    public void refreshFactionMovementBudget(List<BattleUnit> factionsUnits)
    {
        foreach (var unit in factionsUnits)
        {
            unit.ReplenishMovementBudget();
        }
    }

    private IEnumerator RunAITurn()
    {
        foreach (BattleUnit enemy in EnemyUnits)
        {
            if (enemy == null) continue;

            // Replenish movement just in case
            enemy.ReplenishMovementBudget();

            // Wait until no animation is running
            while (BattleGrid.Instance.isAnimating)
                yield return null;

            // Perform AI move
            AIMove(enemy);

            // Wait for this unit to finish moving
            while (enemy.isMoving || BattleGrid.Instance.isAnimating)
                yield return null;
        }

        // After all enemy units have acted, end AI turn
        EndTurn();
    }

    /// <summary>
    /// AI logic for a single enemy unit
    /// </summary>
    private void AIMove(BattleUnit enemy)
    {
        if (enemy == null || enemy.currentTile == null) return;
        BattleGrid grid = BattleGrid.Instance;
        if (grid == null) return;

        // 1. Find closest player unit
        BattleUnit closestPlayer = null;
        int closestDist = int.MaxValue;

        foreach (var player in PlayerUnits)
        {
            if (player == null || player.currentTile == null) continue;
            int dist = grid.GetDistance(enemy.currentTile, player.currentTile);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestPlayer = player;
            }
        }

        if (closestPlayer == null) return;

        // 2. Attempt to attack first (MoveToAttack handles adjacency)
        // Only attempt attack if unit has at least 1 movement point left for the attack
        if (enemy.movementBudget > 0 && grid.MoveToAttack(enemy, closestPlayer, true))
            return;

        // 3. Otherwise move toward closest player
        List<BattleTile> path = grid.GetPath(enemy.currentTile, closestPlayer.currentTile);
        if (path == null || path.Count <= 1) return;

        path.RemoveAt(0); // remove starting tile

        // Reserve 1 movement for a possible attack at the end of the path
        int moveSteps = Mathf.Min(enemy.movementBudget - 1, path.Count);
        if (moveSteps <= 0) return;

        List<BattleTile> movePath = path.GetRange(0, moveSteps);
        grid.MoveUnitTo(enemy, movePath[movePath.Count - 1], false, true);
    }


}
