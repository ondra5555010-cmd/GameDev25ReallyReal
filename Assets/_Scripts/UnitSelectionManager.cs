using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance;

    public BattleUnit selectedUnit;
    public Color highlightColor = Color.green;
    private Color originalColor;

    private void Awake()
    {
        Instance = this;
    }

    public void SelectUnit(BattleUnit unit)
    {
        if (selectedUnit == unit) return;

        DeselectUnit();

        if (unit != null && unit.GetRenderer() != null)
        {
            Renderer rend = unit.GetRenderer();
            originalColor = rend.material.color;
            rend.material.color = highlightColor;
        }

        selectedUnit = unit;

        BattleGrid.Instance.setupReachableTiles(selectedUnit);

        // UI only for player units
        if (unit.playerControlled)
            UIManager.Instance.ShowSelectedUnit(unit);
        else
            UIManager.Instance.HideSelectedUnit();
    }


    public void DeselectUnit()
    {
        if (selectedUnit != null && selectedUnit.GetRenderer() != null)
        {
            Renderer rend = selectedUnit.GetRenderer();
            rend.material.color = originalColor;
        }

        selectedUnit = null;

        BattleGrid.Instance.isSpecialMove = false;
        BattleGrid.Instance.ClearAllAccessible();

        UIManager.Instance.HideSelectedUnit();
    }

    
    private void Update()
    {
        HandleSpecialMoveToggle();
    }

    private void HandleSpecialMoveToggle()
    {
        if (selectedUnit == null) return;

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            selectedUnit.ToggleSpecialMove();
        }
    }

}