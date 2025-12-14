using UnityEngine;
using UnityEngine.UIElements;

public class UnitDisplay
{
    private VisualElement container;
    private Label unitNameLabel;
    private Label unitHpLabel;
    
    private Color normalBg = new Color(1f, 1f, 0.88f); // matches template
    private Color highlightBg = new Color(1f, 0.95f, 0.6f);

    public UnitDisplay(VisualElement root, BattleUnit unit)
    {
        container = root.Q<VisualElement>("container");
        unitNameLabel = root.Q<Label>("unit_name");
        unitHpLabel   = root.Q<Label>("unit_hp");

        // Register UI hover events here
        container.RegisterCallback<MouseEnterEvent>(evt =>
        {
            if (unit.currentTile != null)
                unit.currentTile.OnMouseEnter();
        });
        container.RegisterCallback<MouseLeaveEvent>(evt =>
        {
            if (unit.currentTile != null)
                unit.currentTile.OnMouseExit();
        });
        container.RegisterCallback<MouseDownEvent>(evt =>
        {
            if (unit.currentTile != null)
            {
                unit.currentTile.OnMouseDown();
            }
        });

    }

    public void Initialize(BattleUnit unit)
    {
        unitNameLabel.text = unit.unitName;
        unitHpLabel.text   = $"{unit.currentHitPoints}/{unit.maxHitPoints}";
    }
    
    public void SetHighlight()
    {
        container.style.backgroundColor = highlightBg;
    }

    public void ClearHighlight()
    {
        container.style.backgroundColor = normalBg;
    }

    public void UpdateHp(BattleUnit unit)
    {
        unitHpLabel.text = $"{unit.currentHitPoints}/{unit.maxHitPoints}";

        float ratio = (float)unit.currentHitPoints / unit.maxHitPoints;

        unitHpLabel.style.color = new StyleColor(
            ratio > 0.6f ? Color.green :
            ratio > 0.3f ? Color.orange :
            Color.red
        );
    }
}