using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasController : MonoBehaviour
{
    public Canvas mainMenuCanvas;
    public Canvas mapEditorCanvas;
    public Canvas gameCanvas;

    void Start()
    {
        UpdateCanvasState();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateCanvasState();
    }

    void UpdateCanvasState()
    {
        if (mainMenuCanvas != null)
            mainMenuCanvas.enabled = SceneManager.GetActiveScene().name == "MainMenu";

        if (mapEditorCanvas != null)
            mapEditorCanvas.enabled = SceneManager.GetActiveScene().name == "MapEditor";

        if (gameCanvas != null)
            gameCanvas.enabled = SceneManager.GetActiveScene().name == "GameScene";
    }
}
