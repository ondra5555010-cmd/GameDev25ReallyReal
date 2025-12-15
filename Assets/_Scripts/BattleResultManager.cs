using System.Collections;
using UnityEngine;

public class BattleResultManager : MonoBehaviour
{
    public static BattleResultManager Instance;
    private bool battleEnded = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Tuto metodu volá BattleUnit.KillUnit()
    public void CheckBattleState()
    {
        if (battleEnded) return;

        // Počkáme malou chvíli, než se seznamy aktualizují (pokud se volá před Remove)
        // Ale v KillUnit jsme to volali až PO Remove, takže můžeme kontrolovat hned.
        
        int playerCount = TurnAndUnitsManager.Instance.PlayerUnits.Count;
        int enemyCount = TurnAndUnitsManager.Instance.EnemyUnits.Count;

        // 1. PROHRA
        if (playerCount <= 0)
        {  
            Debug.Log("You LOSE!");
            battleEnded = true;
            StartCoroutine(ShowLoseRoutine());
        }
        // 2. VÝHRA
        else if (enemyCount <= 0)
        {   
            Debug.Log("YOU WIN!");
            battleEnded = true;
            HandleVictoryLoot();
        }
    }

    private void HandleVictoryLoot()
    {
        string enemyID = GameSession.currentEnemyID;
        string lootMessage = "Bitva vyhrána!";
        
        // --- KONTROLA TYPU NEPŘÍTELE ---
        
        // A) Boss / Mimic -> Koruna -> Konec Hry
        if (enemyID != null && (enemyID.Contains("Mimic") || enemyID.Contains("Boss")))
        {
            GameSession.hasRoyalCrown = true;
            lootMessage = "Received item: \n CROWN OF THE DUNGEON KING";
            StartCoroutine(ShowEndingRoutine(lootMessage));
            return; // Ukončíme metodu, jdeme rovnou na ending screen
        }
        
        // B) Skeleton / Minion -> Klíč
        else if (enemyID != null && (enemyID.Contains("Skeleton") || enemyID.Contains("Minion")))
        {
            if (!GameSession.hasDungeonKey)
            {
                GameSession.hasDungeonKey = true;
                lootMessage = "Received item: \n\n KEY OF THE DUNGEON";
            }
            else
            {
                lootMessage = "(Key already owned)";
            }
        }

        // Zobrazíme standardní Win Screen s lootem
        StartCoroutine(ShowWinRoutine(lootMessage));
    }

    // --- ČASOVÁNÍ ZOBRAZENÍ ---

    IEnumerator ShowLoseRoutine()
    {
        yield return new WaitForSeconds(2f); // Pauza na drama
        UIManager.Instance.ShowLoseScreen();
    }

    IEnumerator ShowWinRoutine(string message)
    {
        yield return new WaitForSeconds(1.5f);
        UIManager.Instance.ShowWinScreen(message);
    }

    IEnumerator ShowEndingRoutine(string message)
    {
        yield return new WaitForSeconds(2f);
        UIManager.Instance.ShowEndingScreen(message);
    }
}