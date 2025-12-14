using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleGrid : MonoBehaviour
{
    public static BattleGrid Instance;
    public bool isAnimating = false;
    public bool isSpecialMove = false;

    public int width = 10;
    public int height = 5;
    public float spacing = 4.83f;
    public BattleTile[,] tiles;

    public GameObject tilePrefab;

    // NEW – hero prefabs
    public GameObject wizardPrefab;
    public GameObject roguePrefab;
    public GameObject clericPrefab;
    public GameObject enemySkelletonPrefab;
    public GameObject testUnitModelEnemy;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        tiles = new BattleTile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x * spacing, 0, y * spacing);
                GameObject tileObj = Instantiate(tilePrefab, pos, Quaternion.identity, transform);

                BattleTile battleTile = tileObj.AddComponent<BattleTile>();
                battleTile.Initialize(x, y);

                tiles[x, y] = battleTile;
            }
        }
        
        var w = SpawnUnit<HeroesWizard>(0, 0, true, wizardPrefab);
        var r = SpawnUnit<HeroesRogue>(0, 1, true, roguePrefab);
        var c = SpawnUnit<HeroesCleric>(0, 2, true, clericPrefab);
    
        SpawnUnit<TestUnit>(11, 0, false, enemySkelletonPrefab);
        SpawnUnit<TestUnit>(11, 1, false, enemySkelletonPrefab);
        SpawnUnit<TestUnit>(11, 2, false, enemySkelletonPrefab);
        SpawnUnit<TestUnit>(11, 3, false, enemySkelletonPrefab);
        SpawnUnit<TestUnit>(11, 4, false, enemySkelletonPrefab);

        TurnAndUnitsManager.Instance.refreshFactionUnits(TurnAndUnitsManager.Instance.PlayerUnits);
    }

    public T SpawnUnit<T>(int x, int y, bool isPlayerControlled = true, GameObject model = null) where T : BattleUnit
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return null;

        BattleTile tile = tiles[x, y];
        if (tile.currentUnit != null)
        {
            Debug.LogWarning("Tile already has a unit.");
            return null;
        }

        GameObject unitObj = new GameObject(typeof(T).Name);
        T unit = unitObj.AddComponent<T>();

        tile.AssignUnit(unit);

        if (model == null)
        {
            Debug.LogError("SpawnUnit called without a model prefab!");
            return unit;
        }
        
        GameObject unitModel = model != null ? model : clericPrefab;
        unit.Initialize(unitModel, isPlayerControlled);

        if (isPlayerControlled)
            UIManager.Instance.PopulatePlayerUnits(TurnAndUnitsManager.Instance.PlayerUnits);
        else
            UIManager.Instance.PopulateComputerUnits(TurnAndUnitsManager.Instance.EnemyUnits);

        return unit;
    }

    public void ClearAllAccessible()
    {
        foreach (var tile in tiles)
            tile.unsetAccessible();
    }

    public List<BattleTile> GetNeighbours(BattleTile tile)
    {
        List<BattleTile> n = new List<BattleTile>();
        int x = tile.gridX;
        int y = tile.gridY;

        if (y + 1 < height) n.Add(tiles[x, y + 1]);
        if (y - 1 >= 0) n.Add(tiles[x, y - 1]);
        if (x + 1 < width) n.Add(tiles[x + 1, y]);
        if (x - 1 >= 0) n.Add(tiles[x - 1, y]);

        return n;
    }

    public List<BattleTile> GetReachableTiles(BattleTile start, int movementBudget, BattleUnit movingUnit)
    {
        List<BattleTile> reachable = new List<BattleTile>();
        if (start == null) return reachable;

        Queue<(BattleTile tile, int cost)> queue = new Queue<(BattleTile, int)>();
        HashSet<BattleTile> visited = new HashSet<BattleTile>();

        queue.Enqueue((start, 0));
        visited.Add(start);

        while (queue.Count > 0)
        {
            var (current, cost) = queue.Dequeue();
            reachable.Add(current);
            current.setAccessible();

            if (cost >= movementBudget) continue;

            foreach (var neigh in GetNeighbours(current))
            {
                if (!visited.Contains(neigh))
                {
                    if (neigh.currentUnit != null)
                    {
                        // Only mark hostile tiles if the unit has an action ready
                        if (movingUnit.isActionReady && neigh.IsHostileTile())
                            neigh.setAccessible();
                        continue; // occupied tiles can't be moved into
                    }

                    visited.Add(neigh);
                    queue.Enqueue((neigh, cost + 1));
                }
            }
        }

        // After BFS, mark hostile neighbors of reachable tiles if action is ready
        if (movingUnit.isActionReady)
        {
            foreach (var tile in reachable)
            {
                foreach (var neigh in GetNeighbours(tile))
                {
                    if (neigh.currentUnit != null && neigh.IsHostileTile())
                        neigh.setAccessible();
                }
            }
        }

        return reachable;
    }

    public void setupReachableTiles(BattleUnit movingUnit)
    {
        ClearAllAccessible();
        List<BattleTile> reachable = GetReachableTiles(movingUnit.currentTile, movingUnit.movementBudget, movingUnit);
        foreach (BattleTile tile in reachable)
            tile.setAccessible();
    }

    public int GetDistance(BattleTile a, BattleTile b)
    {
        return Mathf.Abs(a.gridX - b.gridX) + Mathf.Abs(a.gridY - b.gridY);
    }

    public int GetBlockedDistance(BattleTile start, BattleTile target)
    {
        if (start == null || target == null) return -1;

        Queue<(BattleTile tile, int dist)> queue = new Queue<(BattleTile, int)>();
        HashSet<BattleTile> visited = new HashSet<BattleTile>();

        queue.Enqueue((start, 0));
        visited.Add(start);

        while (queue.Count > 0)
        {
            var (current, dist) = queue.Dequeue();

            if (current == target) return dist;

            foreach (var neigh in GetNeighbours(current))
            {
                if (neigh.IsHostileTile() && neigh != target) continue;
                if (neigh.currentUnit != null && neigh != target) continue;

                if (!visited.Contains(neigh))
                {
                    visited.Add(neigh);
                    queue.Enqueue((neigh, dist + 1));
                }
            }
        }

        return -1;
    }

    public bool MoveUnitTo(BattleUnit unit, BattleTile target, bool isForcedMove = false, bool animate = true, float stepDuration = 0.25f, float arcHeight = 0.5f)
    {
        if (unit == null || target == null) return false;
        BattleTile start = unit.currentTile;

        List<BattleTile> path = GetPath(start, target);
        if (path == null || path.Count == 0) return false;

        path.RemoveAt(0); // remove starting tile

        if (path.Count > unit.movementBudget && !isForcedMove)
            return false;

        if (!isForcedMove)
            unit.movementBudget -= path.Count;

        ClearAllAccessible();

        if (animate)
            StartCoroutine(MoveUnitAlongPath(unit, path, stepDuration, arcHeight));
        else
            foreach (BattleTile step in path)
                step.MoveUnitHere(unit);

        unit.RefreshUI();
        return true;
    }

    private IEnumerator MoveUnitAlongPath(BattleUnit unit, List<BattleTile> path, float stepDuration, float arcHeight)
    {
        if (unit == null || path == null || path.Count == 0) yield break;

        isAnimating = true;
        unit.isMoving = true;

        foreach (BattleTile step in path)
        {
            yield return StartCoroutine(step.MoveUnitHereAnimated(unit, step, stepDuration, arcHeight));
        }

        unit.isMoving = false;
        isAnimating = false;

        setupReachableTiles(unit);
    }

    public List<BattleTile> GetPath(BattleTile start, BattleTile target)
    {
        Queue<BattleTile> queue = new Queue<BattleTile>();
        Dictionary<BattleTile, BattleTile> cameFrom = new Dictionary<BattleTile, BattleTile>();
        HashSet<BattleTile> visited = new HashSet<BattleTile>();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            BattleTile current = queue.Dequeue();

            if (current == target)
                return ReconstructPath(cameFrom, target);

            foreach (var neigh in GetNeighbours(current))
            {
                if (neigh.IsHostileTile() && neigh != target) continue;
                if (neigh.currentUnit != null && neigh != target) continue;

                if (!visited.Contains(neigh))
                {
                    visited.Add(neigh);
                    cameFrom[neigh] = current;
                    queue.Enqueue(neigh);
                }
            }
        }

        return null;
    }

    private List<BattleTile> ReconstructPath(Dictionary<BattleTile, BattleTile> cameFrom, BattleTile end)
    {
        List<BattleTile> path = new List<BattleTile>();
        BattleTile current = end;

        path.Add(current);
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }

        path.Reverse();
        return path;
    }

    public bool MoveToAttack(BattleUnit attacker, BattleUnit defender, bool animate = true)
    {
        if (attacker == null || defender == null) return false;
        if (!attacker.isActionReady) return false; // cannot attack if action is spent

        BattleTile defenderTile = defender.currentTile;
        if (defenderTile == null) return false;

        // Already adjacent
        if (GetNeighbours(defenderTile).Contains(attacker.currentTile))
        {
            StartCoroutine(AttackAfterMovement(attacker, defender));
            return true;
        }

        // Find candidate tiles to move to (adjacent to defender)
        List<BattleTile> candidates = new List<BattleTile>();
        foreach (BattleTile neigh in GetNeighbours(defenderTile))
        {
            if (neigh.currentUnit != null) continue;
            candidates.Add(neigh);
        }

        if (candidates.Count == 0) return false;

        // Find the reachable candidate tile closest to attacker
        BattleTile bestTile = null;
        int bestDist = int.MaxValue;
        foreach (var tile in candidates)
        {
            int d = GetBlockedDistance(attacker.currentTile, tile);
            if (d >= 0 && d < bestDist && d <= attacker.movementBudget)
            {
                bestDist = d;
                bestTile = tile;
            }
        }

        if (bestTile == null) return false;

        if (!MoveUnitTo(attacker, bestTile, false, animate)) return false;

        StartCoroutine(AttackAfterMovement(attacker, defender));

        return true;
    }

    private IEnumerator AttackAfterMovement(BattleUnit attacker, BattleUnit defender)
    {
        isAnimating = true;

        while (attacker.isMoving)
            yield return null;

        attacker.Attack(defender);

        setupReachableTiles(attacker);

        isAnimating = false;
    }
    
    public void RangedTargets(bool targetPlayers)
    {
        // 1) Reset tile accessibility
        ClearAllAccessible();

        if (!UnitSelectionManager.Instance.selectedUnit.isActionReady)
        {
            return;
        }

        // 2) Mark tiles containing target-faction units as accessible
        List<BattleUnit> targets = targetPlayers
            ? TurnAndUnitsManager.Instance.PlayerUnits
            : TurnAndUnitsManager.Instance.EnemyUnits;

        foreach (var unit in targets)
        {
            if (unit == null || unit.currentTile == null) continue;
            unit.currentTile.setAccessible();
        }
    }
    
    public List<BattleUnit> GetNeighbouringUnits(BattleTile tile)
    {
        List<BattleUnit> units = new List<BattleUnit>();
        if (tile == null) return units;

        foreach (var neigh in GetNeighbours(tile))
        {
            if (neigh.currentUnit != null)
                units.Add(neigh.currentUnit);
        }

        return units;
    }
}
