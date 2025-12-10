using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnAndUnitsManager : MonoBehaviour
{
    public static TurnAndUnitsManager Instance;
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
            refreshFactionUnits(PlayerUnits);
        }
        else
        {
            refreshFactionUnits(EnemyUnits);
            StartCoroutine(RunAITurn());
        }
    }
    
    public void refreshFactionUnits(List<BattleUnit> factionsUnits)
    {
        foreach (var unit in factionsUnits)
        {
            unit.ReplenishMovementBudget();
            unit.isActionReady = true;
        }
    }

    private IEnumerator RunAITurn()
    {
        foreach (BattleUnit enemy in EnemyUnits)
        {
            if (enemy == null) continue;

            // Wait until no animation is running
            while (BattleGrid.Instance.isAnimating || UIManager.Instance.isDisplaying)
                yield return null;

            // Perform AI move
            AIMove(enemy);

            // Wait for this unit to finish moving
            while (enemy.isMoving || BattleGrid.Instance.isAnimating || UIManager.Instance.isDisplaying)
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

        // 2. Attempt to attack first (only if action is ready)
        if (enemy.isActionReady && grid.MoveToAttack(enemy, closestPlayer, true))
            return;

        // 3. Otherwise move toward closest player
        List<BattleTile> path = grid.GetPath(enemy.currentTile, closestPlayer.currentTile);
        if (path == null || path.Count <= 1) return;

        path.RemoveAt(0); // remove starting tile

        int moveSteps = Mathf.Min(enemy.movementBudget, path.Count);
        if (moveSteps <= 0) return;

        List<BattleTile> movePath = path.GetRange(0, moveSteps);
        grid.MoveUnitTo(enemy, movePath[movePath.Count - 1], false, true);
    }
}
