using UnityEngine.UIElements;

public class UnitDisplay
{
    private Label unitNameLabel;
    private Label unitHpLabel;

    public UnitDisplay(VisualElement root)
    {
        unitNameLabel = root.Q<Label>("unit_name");
        unitHpLabel   = root.Q<Label>("unit_hp");
    }

    public void Initialize(BattleUnit unit)
    {
        unitNameLabel.text = unit.unitName;
        unitHpLabel.text   = $"{unit.currentHitPoints}/{unit.maxHitPoints}";
    }

    public void UpdateHp(BattleUnit unit)
    {
        unitHpLabel.text = $"{unit.currentHitPoints}/{unit.maxHitPoints}";
    }
}