using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OnAdminButtonClick()
    {
        SceneManager.LoadScene("MapEditor", LoadSceneMode.Single);
    }

    public void OnPlayerButtonClick()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
}
