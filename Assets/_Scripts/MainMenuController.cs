using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;   // UI_panel (title + buttons)
    [SerializeField] private GameObject hintPanel;   // Hint_panel (overlay)

    [Header("Scene To Load")]
    [SerializeField] private string gameSceneName = "DungeonScene";

    private void Start()
    {
        // Menu start: hint schovat, main ukázat
        if (hintPanel != null) hintPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);

        // ESC tady nechceme řešit (nic nepauzovat)
        Time.timeScale = 1f;
    }

    public void OnPlay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnHint()
    {
        if (hintPanel != null) hintPanel.SetActive(true);
        if (mainPanel != null) mainPanel.SetActive(false);
    }

    public void OnBack()
    {
        if (hintPanel != null) hintPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);
    }

    public void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
