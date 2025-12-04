using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleTile : MonoBehaviour
{
    public int gridX;
    public int gridY;
    private Renderer tileRenderer;
    public BattleUnit currentUnit;
    public bool Accessible;
    private bool isMouseOver;
    
    private Color originalColor;
    public Color highlightColor = Color.yellow;
    public Color pathfindingColor = Color.green;
    public Color hostileColor = Color.red;

    public void Initialize(int x, int y)
    {
        gridX = x;
        gridY = y;
        tileRenderer = GetComponent<Renderer>();
        originalColor = tileRenderer.material.color;
    }

    public void setAccessible()
    {
        Accessible = true;

        if (IsHostileTile())
        {
            tileRenderer.material.color = hostileColor;
            return;
        }

        if (isMouseOver)
        {
            tileRenderer.material.color = highlightColor;
            return;
        }

        tileRenderer.material.color = pathfindingColor;
    }


    public void unsetAccessible()
    {
        Accessible = false;

        if (isMouseOver)
        {
            tileRenderer.material.color = highlightColor;
            return;
        }
        
        tileRenderer.material.color = originalColor;
    }


    
    public bool AssignUnit(BattleUnit unit)
    {
        if (currentUnit != null)
        {
            return false;
        }
        currentUnit = unit;
        unit.currentTile = this;
        return true;
    }

    public void ClearUnit()
    {
        if (currentUnit != null)
        {
            currentUnit.currentTile = null;
            currentUnit = null;
        }
    }
    
    void OnMouseEnter()
    {
        isMouseOver = true;
        if (IsHostileTile())
            tileRenderer.material.color = hostileColor;
        else
            tileRenderer.material.color = highlightColor;

    }

    void OnMouseExit()
    {
        isMouseOver = false;

        if (Accessible)
        {
            if (currentUnit != null && currentUnit.playerControlled != TurnAndUnitManager.Instance.isPlayerTurn)
                tileRenderer.material.color = hostileColor; // red if accessible and hostile
            else
                tileRenderer.material.color = pathfindingColor; // green if accessible and not hostile
        }
        else
        {
            tileRenderer.material.color = originalColor; // grey/original if not accessible
        }
    }

    
    void OnMouseDown()
    {
        if (!TurnAndUnitManager.Instance.isPlayerTurn) return;
        
        if (BattleGrid.Instance.isAnimating)
        {
            return;
        }

        var sm = UnitSelectionManager.Instance;
        var selected = sm.selectedUnit;

        Debug.Log($"MouseDown on tile ({gridX},{gridY})");
        if (currentUnit != null)
        {
            if (currentUnit.playerControlled)
            {
                Debug.Log("Selecting unit on this tile");
                sm.SelectUnit(currentUnit);
            }
            else if (selected != null && selected.playerControlled)
            {
                Debug.Log($"Attacking hostile unit {currentUnit.name} with selected unit {selected.name}");
                BattleGrid.Instance.MoveToAttack(selected, currentUnit, animate: true);
            }
            else
            {
                Debug.Log("Cannot select or attack this unit");
            }

            return; // stop here, hostile units cannot be selected
        }

        // If tile is empty and a unit is selected, move it there
        if (selected != null)
        {
            Debug.Log($"Moving selected unit {selected.name} to empty tile ({gridX},{gridY})");
            BattleGrid.Instance.MoveUnitTo(selected, this, false, true, 0.25f, 0.5f);
        }
    }
    
    public void MoveUnitHere(BattleUnit unit)
    {
        /*if (!ForcedMove)
        {
            unit.movementBudget -= BattleGrid.Instance.GetBlockedDistance(unit.currentTile, this);
        }*/
        
        if (unit.currentTile != null)
            unit.currentTile.currentUnit = null;

        AssignUnit(unit);
        unit.UpdatePosition();
    }
    
    public IEnumerator MoveUnitHereAnimated(BattleUnit unit, BattleTile targetTile, float stepDuration, float arcHeight)
    {
        if (unit == null || targetTile == null) yield break;

        BattleTile startTile = unit.currentTile;

        // Detach from old tile immediately
        if (startTile != null)
            startTile.currentUnit = null;

        Vector3 from = unit.transform.position;
        Vector3 to = targetTile.transform.position + Vector3.up * 0.5f;

        float elapsed = 0f;

        while (elapsed < stepDuration)
        {
            float t = Mathf.Clamp01(elapsed / stepDuration);
            float s = t * t * (3f - 2f * t); // smoothstep

            Vector3 pos = Vector3.Lerp(from, to, s);
            pos.y += Mathf.Sin(t * Mathf.PI) * arcHeight;

            unit.transform.position = pos;

            elapsed += Time.deltaTime;
            yield return null;
        }
        
        unit.transform.position = to;
        
        targetTile.AssignUnit(unit);
    }
    
    public bool IsHostileTile()
    {
        if (currentUnit == null) return false;

        return currentUnit.playerControlled != TurnAndUnitManager.Instance.isPlayerTurn;
    }



}
