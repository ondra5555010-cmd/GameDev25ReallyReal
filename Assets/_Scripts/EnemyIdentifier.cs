using UnityEngine;

public class EnemyIdentifier : MonoBehaviour
{
    [Tooltip("Unikátní ID nepřítele (např. Goblin_Forest_1)")]
    public string enemyID;

    void Start()
    {
        // Pokud je toto ID na seznamu mrtvých v GameSession, znič se
        if (GameSession.deadEnemies.Contains(enemyID))
        {
            Destroy(gameObject);
        }
    }
}