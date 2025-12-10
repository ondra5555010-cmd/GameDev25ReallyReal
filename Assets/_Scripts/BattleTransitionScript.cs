using UnityEngine;
using UnityEngine.SceneManagement;

public class Battletransitionscript : MonoBehaviour
{
    // Jméno battle scény
    [SerializeField] private string battleSceneName = "BattleScene";

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something collides.");
        // Ověříme, jestli je objekt nepřítel
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy hit! Loading battle scene...");

            // Načtení battle scény
            SceneManager.LoadScene(battleSceneName);
        }
    }
}