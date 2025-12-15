using UnityEngine;
using UnityEngine.SceneManagement;

public class Battletransitionscript : MonoBehaviour
{
    [SerializeField] private string battleSceneName = "BattleScene";

    private void Start()
    {
        // Pokud se vracíme z boje, obnovíme pozici hrdinů
        if (GameSession.returningFromBattle)
        {
            transform.position = GameSession.lastPlayerPosition;
            GameSession.returningFromBattle = false; // Reset flagu
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something collides.");
        // 1. Zkontrolujeme tag
        if (other.CompareTag("Enemy"))
        {
            // 2. Pokusíme se získat identifikační skript z objektu, do kterého jsme narazili
            EnemyIdentifier enemy = other.GetComponent<EnemyIdentifier>();

            if (enemy != null)
            {
                // Pokud je tento nepřítel už na seznamu mrtvých...
                if (GameSession.deadEnemies.Contains(enemy.enemyID))
                {
                    Debug.Log($"Ignoruji kolizi s mrtvým nepřítelem: {enemy.enemyID}");
                    
                    // Můžeme ho rovnou zničit, aby už neprudil
                    Destroy(other.gameObject); 
                    
                    // A okamžitě vyskočíme z metody, aby se nenačetla scéna
                    return; 
                }
                Debug.Log($"Hit enemy: {enemy.enemyID}. Loading battle...");

                // 3. ULOŽÍME DATA DO GAMESESSION
                GameSession.currentEnemyID = enemy.enemyID;
                GameSession.lastPlayerPosition = transform.position;
                GameSession.returningFromBattle = true;

                // 4. Načteme boj
                SceneManager.LoadScene(battleSceneName);
            }
            else
            {
                Debug.LogError("Narazil jsem do objektu s tagem Enemy, ale chybí mu skript EnemyIdentifier!");
            }
        }
    }
}
