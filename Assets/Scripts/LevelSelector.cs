using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public Dropdown levelDropdown;
    public Button loadLevelButton;

    void Start()
    {
        PopulateLevelDropdown();
        loadLevelButton.onClick.AddListener(OnLoadLevelButtonClick);
    }

    void PopulateLevelDropdown()
    {
        levelDropdown.ClearOptions();
        List<string> mapNames = MapSaveLoad.GetSavedMaps();
        levelDropdown.AddOptions(mapNames);
    }

    void OnLoadLevelButtonClick()
    {
        string selectedLevel = levelDropdown.options[levelDropdown.value].text;
        PlayerPrefs.SetString("SelectedLevel", selectedLevel);
        SceneManager.LoadScene("GameScene");
    }
}
