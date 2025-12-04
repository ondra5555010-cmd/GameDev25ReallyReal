using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleGrid : MonoBehaviour
{
    public static BattleGrid Instance;
    public bool isAnimating = false;

    
    public int width = 10;
    public int height = 5;
    public float spacing = 4.83f;
    public BattleTile[,] tiles;
    
    public GameObject tilePrefab;
    public GameObject floatingTextPrefab;
    public GameObject testUnitModel;
    public GameObject testUnitModelEnemy;

    void Awake()
    {
        // Singleton pattern
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
        
        SpawnUnit<TestUnit>(0, 0);
        SpawnUnit<TestUnit>(0, 1);
        SpawnUnit<TestUnit>(0, 2);
        SpawnUnit<TestUnit>(11, 0, false, testUnitModelEnemy);
        SpawnUnit<TestUnit>(11, 1, false, testUnitModelEnemy);
        SpawnUnit<TestUnit>(11, 2, false, testUnitModelEnemy);
        SpawnUnit<TestUnit>(11, 3, false, testUnitModelEnemy);
        SpawnUnit<TestUnit>(11, 4, false, testUnitModelEnemy);

        TurnAndUnitManager.Instance.refreshFactionMovementBudget(TurnAndUnitManager.Instance.PlayerUnits);
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
        
        unit.floatingTextPrefab = floatingTextPrefab;

        tile.AssignUnit(unit);

        GameObject unitModel = model != null ? model : testUnitModel;
        unit.Initialize(unitModel, isPlayerControlled);

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

    public List<BattleTile> GetReachableTiles(BattleTile start, int movementBudget)
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

            if (cost >= movementBudget) continue;

            foreach (var neigh in GetNeighbours(current))
            {
                if (neigh.currentUnit != null && neigh != start)
                {
                    if (neigh.IsHostileTile() && cost + 1 <= movementBudget)
                        neigh.setAccessible();
                    continue;
                }

                if (neigh.currentUnit != null && neigh != start) continue;

                if (!visited.Contains(neigh))
                {
                    visited.Add(neigh);
                    queue.Enqueue((neigh, cost + 1));
                }
            }
        }

        return reachable;
    }

    public void setupReachableTiles(BattleUnit movingUnit)
    {
        ClearAllAccessible();
        List<BattleTile> reachable = GetReachableTiles(movingUnit.currentTile, movingUnit.movementBudget);
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
        BattleTile defenderTile = defender.currentTile;
        if (defenderTile == null) return false;

        // 1. Adjacent attack
        if (GetNeighbours(defenderTile).Contains(attacker.currentTile))
        {
            if (attacker.movementBudget < 1) return false; // need 1 point to attack
            attacker.movementBudget -= 1;
            StartCoroutine(AttackAfterMovement(attacker, defender));
            return true;
        }

        // 2. Find candidate tiles to move
        List<BattleTile> candidates = new List<BattleTile>();
        foreach (BattleTile neigh in GetNeighbours(defenderTile))
        {
            if (neigh.currentUnit != null) continue;
            if (neigh.IsHostileTile()) continue;
            candidates.Add(neigh);
        }

        if (candidates.Count == 0) return false;

        // 3. Choose best reachable tile
        BattleTile bestTile = null;
        int bestDist = int.MaxValue;

        foreach (var tile in candidates)
        {
            int d = GetBlockedDistance(attacker.currentTile, tile);
            if (d >= 0 && d < bestDist)
            {
                bestDist = d;
                bestTile = tile;
            }
        }

        if (bestTile == null) return false;

        // 4. Ensure enough movement points for move + attack
        int totalCost = bestDist + 1;
        if (attacker.movementBudget < totalCost) return false;

        // 5. Move unit
        if (!MoveUnitTo(attacker, bestTile, false, animate)) return false;

        // 6. Deduct total cost (move + attack)
        attacker.movementBudget -= totalCost;

        StartCoroutine(AttackAfterMovement(attacker, defender));
        return true;
    }

    private IEnumerator AttackAfterMovement(BattleUnit attacker, BattleUnit defender)
    {
        isAnimating = true;
    
        // Wait until attacker finishes moving
        while (attacker.isMoving)
            yield return null;

        // Perform combat instead of instant kill
        attacker.Attack(defender);

        // Update reachable tiles
        setupReachableTiles(attacker);

        isAnimating = false;
    }
}
