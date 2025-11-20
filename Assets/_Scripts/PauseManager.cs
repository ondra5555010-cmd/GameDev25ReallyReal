using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panelDim;
    public GameObject panelMenu;
    public Button resumeButton;
    public Button quitButton;
    public GameObject firstSelected;

    public static bool IsPaused { get; private set; }

    void Start()
    {
        SetUI(false);
        resumeButton.onClick.AddListener(ResumeGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused) ResumeGame();
            else PauseGame();
        }
    }

    void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;

        SetUI(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;

        SetUI(false);
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void SetUI(bool state)
    {
        panelDim.SetActive(state);
        panelMenu.SetActive(state);
    }
}
