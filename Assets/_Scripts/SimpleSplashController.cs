using UnityEngine;
using UnityEngine.SceneManagement; // Potřeba pro načítání scén
using System.Collections; // Potřeba pro Coroutines (časovače)

public class SimpleSplashController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Název scény s hlavním menu")]
    [SerializeField] private string sceneToLoad = "MainMenuScene";
    
    [Tooltip("Celková doba trvání splash screenu (musí odpovídat délce animace)")]
    public float totalDuration = 3.0f;

    void Start()
    {
        // Spustíme odpočet hned při startu scény
        StartCoroutine(WaitAndLoadRoutine());
    }

    // Toto je "Korutina" - speciální metoda, která umí čekat
    IEnumerator WaitAndLoadRoutine()
    {
        // Počkáme přesně tak dlouho, jak trvá animace
        yield return new WaitForSeconds(totalDuration);
        
        // Po uplynutí času načteme další scénu
        SceneManager.LoadSceneAsync(sceneToLoad);
    }
}