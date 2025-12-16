using UnityEngine;

public class DungeonDoor : MonoBehaviour
{
    [Header("Settings")]
    public bool isOpen = false;
    
    // Odkaz na vizuální část dveří (to co se má otočit nebo zmizet)
    public Transform doorVisuals; 

    private void Start()
    {
        // Ujistíme se, že stav odpovídá proměnné
        UpdateVisuals();
    }

    // Tuto metodu zavolá hráč, když zmáčkne L
    public bool TryOpen()
    {
        if (isOpen) return false; // Už je otevřeno

        // Kontrola klíče v GameSession (z předchozích kroků)
        if (GameSession.hasDungeonKey)
        {
            OpenDoor();
            return true;
        }
        else
        {
            Debug.Log("Dveře jsou zamčené. Potřebuješ klíč!");
            // Zde by se hodilo: UIManager.Instance.ShowFloatingText("Need Key!", Color.red ...);
            return false;
        }
    }

    public void OpenDoor()
    {
        isOpen = true;
        UpdateVisuals();
        Debug.Log("Dveře odemčeny!");
    }

    private void UpdateVisuals()
    {
        if (isOpen)
        {
            // MOŽNOST A: Dveře zmizí (deaktivují se)
            // gameObject.SetActive(false); 
            
            // MOŽNOST B: Dveře se otočí (např. o 90 stupňů)
            if (doorVisuals != null)
                doorVisuals.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            if (doorVisuals != null)
                doorVisuals.localRotation = Quaternion.Euler(0, 90, 0);
        }
    }
}