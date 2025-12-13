using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    private UIDocument uiDocument;
    private VisualElement root;
    private VisualElement playerUnitsContainer;
    private VisualElement computerUnitsContainer;
    private VisualElement selectedUnitDisplay;
    
    public VisualTreeAsset playerUnitTemplate;

    public GameObject floatingTextPrefab;
    public float displayDelay = 5f; // time between queued texts
    public bool isDisplaying = false;
    
    private Label selectedAbilityDescription;
    private Label selectedName;
    private Label selectedHp;
    private Label selectedAC;
    private Label selectedDmg;
    private Label selectedAtk;
    private Label selectedMp;
    
    private Queue<FloatingTextRequest> queueTextRequest = new();
    
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        playerUnitsContainer   = root.Q<VisualElement>("player_units_display");
        computerUnitsContainer = root.Q<VisualElement>("computer_units_display");
        selectedUnitDisplay = root.Q<VisualElement>("selected_unit_display");
        
        selectedAbilityDescription = selectedUnitDisplay.Q<Label>("special_ability_description");
        selectedName = selectedUnitDisplay.Q<Label>("unit_name");
        selectedHp = selectedUnitDisplay.Q<Label>("unit_hp");
        selectedAC = selectedUnitDisplay.Q<Label>("unit_AC");
        selectedDmg = selectedUnitDisplay.Q<Label>("unit_dmg");
        selectedAtk = selectedUnitDisplay.Q<Label>("unit_attk");
        selectedMp = selectedUnitDisplay.Q<Label>("unit_mp");
    }
    
    void Start()
    {
        
    }


    public void ShowFloatingText(string message, Color color, Transform target, Vector3 offset, List<System.Action> onShowActions = null)
    {
        queueTextRequest.Enqueue(new FloatingTextRequest(message, color, target, offset, onShowActions));

        if (!isDisplaying)
            StartCoroutine(ProcessQueue());
    }


    private IEnumerator ProcessQueue()
    {
        isDisplaying = true;

        while (queueTextRequest.Count > 0)
        {
            var request = queueTextRequest.Dequeue();

            GameObject go = Instantiate(floatingTextPrefab, request.target.position + request.offset, Quaternion.identity);
            var ft = go.GetComponent<FloatingTextScript>() ?? go.GetComponentInChildren<FloatingTextScript>();
            if (ft != null)
                ft.Show(request.message, request.color, request.offset, request.target);
            
            foreach (var action in request.onShowActions)
            {
                action?.Invoke();
            }

            yield return new WaitForSeconds(displayDelay);
        }

        isDisplaying = false;
    }

    private class FloatingTextRequest
    {
        public string message;
        public Color color;
        public Transform target;
        public Vector3 offset;
        public List<System.Action> onShowActions; // new list of actions

        public FloatingTextRequest(string message, Color color, Transform target, Vector3 offset, List<System.Action> onShowActions = null)
        {
            this.message = message;
            this.color = color;
            this.target = target;
            this.offset = offset;
            this.onShowActions = onShowActions ?? new List<System.Action>();
        }
    }
    
    public void PopulatePlayerUnits(List<BattleUnit> units)
    {
        Populate(units, playerUnitsContainer);
    }

    public void PopulateComputerUnits(List<BattleUnit> units)
    {
        Populate(units, computerUnitsContainer);
    }
    
    private void Populate(List<BattleUnit> units, VisualElement container)
    {
        if (container == null) return;

        container.Clear();

        foreach (var unit in units)
        {
            var ve = playerUnitTemplate.CloneTree();
            var display = new UnitDisplay(ve, unit); // <-- pass the unit here

            display.Initialize(unit);

            container.Add(ve);

            unit.uiDisplay = display;
        }
    }

    
    public void ShowSelectedUnit(BattleUnit unit)
    {
        if (unit == null) return;

        selectedUnitDisplay.style.display = DisplayStyle.Flex;

        selectedName.text = unit.unitName;
        selectedAbilityDescription.text = unit.specialAbilityDescription; // <-- set description
        selectedHp.text   = $"HP: {unit.currentHitPoints}/{unit.maxHitPoints}";
        selectedAC.text   = $"AC: {unit.armorClass}";
        selectedDmg.text  = $"DMG: {unit.damageDice}d{unit.damageDie}";
        selectedAtk.text  = $"ATK: +{unit.attackBonus}";
        string actionMarker = unit.isActionReady ? "*" : "";
        selectedMp.text   = $"MP: {unit.movementBudget}/{unit.maxMovementBudget}{actionMarker}";
    }


    public void HideSelectedUnit()
    {
        selectedUnitDisplay.style.display = DisplayStyle.None;
    }

}