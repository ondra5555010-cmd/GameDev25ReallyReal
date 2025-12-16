using UnityEngine;
using UnityEngine.UIElements;

public class UnitDisplay
{
    private VisualElement container;
    private Label unitNameLabel;
    private Label unitHpLabel;
    private Label movementLabel;
    private Label actionLabel;

    private Color normalBg = new Color(1f, 1f, 0.88f);
    private Color highlightBg = new Color(1f, 0.95f, 0.6f);

    public UnitDisplay(VisualElement root, BattleUnit unit)
    {
        container      = root.Q<VisualElement>("container");
        unitNameLabel  = root.Q<Label>("unit_name");
        unitHpLabel    = root.Q<Label>("unit_hp");
        movementLabel  = root.Q<Label>("movement_points");
        actionLabel    = root.Q<Label>("action");

        // Player-only visibility
        bool isPlayer = unit.playerControlled;

        movementLabel.style.display = isPlayer ? DisplayStyle.Flex : DisplayStyle.None;
        actionLabel.style.display   = isPlayer ? DisplayStyle.Flex : DisplayStyle.None;

        // Hover / click forwarding
        container.RegisterCallback<MouseEnterEvent>(_ =>
        {
            if (unit.currentTile != null)
                unit.currentTile.OnMouseEnter();
        });

        container.RegisterCallback<MouseLeaveEvent>(_ =>
        {
            if (unit.currentTile != null)
                unit.currentTile.OnMouseExit();
        });

        container.RegisterCallback<MouseDownEvent>(_ =>
        {
            if (unit.currentTile != null)
                unit.currentTile.OnMouseDown();
        });
    }

    public void Initialize(BattleUnit unit)
    {
        unitNameLabel.text = unit.unitName;
        UpdateDisplay(unit);
    }

    public void UpdateDisplay(BattleUnit unit)
    {
        // HP
        unitHpLabel.text = $"{unit.currentHitPoints}/{unit.maxHitPoints}";

        float ratio = (float)unit.currentHitPoints / unit.maxHitPoints;
        unitHpLabel.style.color = ratio > 0.6f ? Color.green :
                                  ratio > 0.3f ? Color.yellow :
                                  Color.red;

        // Player-only info
        if (unit.playerControlled)
        {
            movementLabel.text = $"{unit.movementBudget} MP";
            actionLabel.text   = unit.isActionReady ? "Action: READY" : "Action: USED";

            actionLabel.style.color = unit.isActionReady
                ? Color.cadetBlue
                : Color.orange;
        }
    }

    public void SetHighlight()
    {
        container.style.backgroundColor = highlightBg;
    }

    public void ClearHighlight()
    {
        container.style.backgroundColor = normalBg;
    }
}
