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
    public Color enemyHostiletColor = Color.darkRed;


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
        ApplyColorState();
    }



    public void unsetAccessible()
    {
        Accessible = false;
        ApplyColorState();
    }



    
    public bool AssignUnit(BattleUnit unit)
    {
        if (currentUnit != null)
        {
            return false;
        }

        currentUnit = unit;
        unit.currentTile = this;
        ApplyColorState();
        return true;
    }

    public void ClearUnit()
    {
        if (currentUnit != null)
        {
            currentUnit.currentTile = null;
            currentUnit = null;
        }
        ApplyColorState();
    }
    
    void OnMouseEnter()
    {
        isMouseOver = true;
        ApplyColorState();
    }

    void OnMouseExit()
    {
        isMouseOver = false;
        ApplyColorState();
    }
    
    void OnMouseDown()
    {
        if (!TurnAndUnitsManager.Instance.isPlayerTurn) return;
        
        if (BattleGrid.Instance.isAnimating)
        {
            return;
        }

        var sm = UnitSelectionManager.Instance;
        var selected = sm.selectedUnit;

        Debug.Log($"MouseDown on tile ({gridX},{gridY})");
        if (currentUnit != null)
        {
            // 1) Priority: special move mode + accessible tile
            if (selected != null && selected.isActionReady)
            {
                // Only act if selected unit has special move active and tile is accessible
                if (BattleGrid.Instance.isSpecialMove && Accessible)
                {
                    Debug.Log($"Enacting special move on tile ({gridX},{gridY})");
                    selected.EnactSpecialMove(currentUnit);
                    return; // stop further processing
                }
            }
            if (currentUnit.playerControlled)
            {
                Debug.Log("Selecting unit on this tile");
                sm.SelectUnit(currentUnit);
            }
            else if (selected != null && selected.playerControlled && selected.isActionReady)
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

        return currentUnit.playerControlled != TurnAndUnitsManager.Instance.isPlayerTurn;
    }
    
    private void ApplyColorState()
    {
        bool playerTurn = TurnAndUnitsManager.Instance.isPlayerTurn;

        // If it's not player turn, always revert to original color
        if (!playerTurn)
        {
            tileRenderer.material.color = originalColor;
            return;
        }

        // If tile contains the selected unit, always highlight
        var selected = UnitSelectionManager.Instance.selectedUnit;
        if (currentUnit != null && selected == currentUnit && Accessible)
        {
            tileRenderer.material.color = highlightColor;
            return;
        }

        if (isMouseOver)
        {
            if (IsHostileTile())
                tileRenderer.material.color = enemyHostiletColor;
            else
                tileRenderer.material.color = highlightColor;
            
            return;
        }
        
        if (Accessible)
        {
            if (IsHostileTile())
                tileRenderer.material.color = hostileColor;
            else
                tileRenderer.material.color = pathfindingColor;

            return;
        }
        
        tileRenderer.material.color = originalColor;
    }
}
