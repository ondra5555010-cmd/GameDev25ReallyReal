using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    private UIDocument uiDocument;
    private VisualElement root;
    private VisualElement unitsContainer;
    public VisualTreeAsset playerUnitTemplate;

    public GameObject floatingTextPrefab;
    public float displayDelay = 5f; // time between queued texts
    public bool isDisplaying = false;
    
    private Queue<FloatingTextRequest> queueTextRequest = new();
    

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        unitsContainer = root.Q<VisualElement>("player_units_display");
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


    
    public void PopulateUnits(List<BattleUnit> units)
    {
        Debug.Log("PopulateUnits");
        Debug.Log(units.Count);
        if (unitsContainer == null) return;

        unitsContainer.Clear();

        foreach (var unit in units)
        {
            var ve = playerUnitTemplate.CloneTree();
            var display = new UnitDisplay(ve);

            display.Initialize(unit);

            unitsContainer.Add(ve);

            // store reference inside the unit for later HP updates
            unit.uiDisplay = display;
        }
    }
}