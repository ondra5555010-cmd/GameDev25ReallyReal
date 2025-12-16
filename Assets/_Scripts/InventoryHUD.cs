using UnityEngine;
using UnityEngine.UI; // Potřeba pro práci s UI prvky

public class InventoryHUD : MonoBehaviour
{
    [Header("UI Elements")]
    // Sem přetáhnete GameObject (Image) klíče, který jste si připravil v Canvasu
    public GameObject keyIconObject; 
    
    void Awake()
    {
        //GameSession.hasDungeonKey = true;
    }
    void Update()
    {
        if (keyIconObject == null) return;

        // Jednoduchá logika:
        // Pokud máme klíč (GameSession.hasDungeonKey je true) -> Zobraz ikonu
        // Pokud klíč nemáme -> Skryj ikonu
        
        // Poznámka: Abychom nevolali SetActive zbytečně každý frame, 
        // můžeme zkontrolovat, zda se stav liší, ale pro jeden objekt to výkon nezatíží.
        if (keyIconObject.activeSelf != GameSession.hasDungeonKey)
        {
            keyIconObject.SetActive(GameSession.hasDungeonKey);
        }
    }
}