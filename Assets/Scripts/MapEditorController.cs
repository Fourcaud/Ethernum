using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapEditorController : MonoBehaviour
{
    public GameObject tilePrefab;
    public Sprite zone1Sprite;
    public Sprite zone2Sprite;
    public Sprite zone3Sprite;
    public Sprite bossSprite;
    public Sprite shopSprite;
    public float cameraSpeed = 5f;
    public InputField saveInputField;
    public Dropdown loadDropdown;
    public Button deleteButton;
    public Button loadButton;

    public Sprite currentSprite;
    private bool isDeleteMode;
    private LayerMask tileLayerMask;
    private List<GameObject> placedTiles = new List<GameObject>();
    private float deleteButtonHoldTime = 0f;
    private float loadButtonHoldTime = 0f;
    private const float deleteButtonHoldThreshold = 2f; // 2 seconds
    private const float loadButtonHoldThreshold = 5f; // 5 seconds

    void Start()
    {
        currentSprite = null;
        isDeleteMode = false;
        tileLayerMask = LayerMask.GetMask("Tile");

        PopulateLoadDropdown();
        deleteButton.onClick.AddListener(OnDeleteButtonClick);
    }

    public void OnZone1ButtonClick()
    {
        currentSprite = zone1Sprite;
        isDeleteMode = false;
    }

    public void OnZone2ButtonClick()
    {
        currentSprite = zone2Sprite;
        isDeleteMode = false;
    }

    public void OnZone3ButtonClick()
    {
        currentSprite = zone3Sprite;
        isDeleteMode = false;
    }

    public void OnBossButtonClick()
    {
        currentSprite = bossSprite;
        isDeleteMode = false;
    }

    public void OnShopButtonClick()
    {
        currentSprite = shopSprite;
        isDeleteMode = false;
    }

    public void OnDeleteButtonClick()
    {
        isDeleteMode = true;
        currentSprite = null;
        deleteButtonHoldTime = 0f; // Reset the hold time
        Debug.Log("Delete mode activated");
    }

    public void OnSaveButtonClick()
    {
        string mapName = saveInputField.text;
        if (!string.IsNullOrEmpty(mapName))
        {
            SaveMap(mapName);
            PopulateLoadDropdown();
        }
        else
        {
            Debug.LogWarning("Map name cannot be empty!");
        }
    }

    public void OnLoadButtonClick()
    {
        string mapName = loadDropdown.options[loadDropdown.value].text;
        LoadMap(mapName);
    }

    void Update()
    {
        HandleCameraMovement();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }

        if (isDeleteMode && Input.GetMouseButton(0))
        {
            deleteButtonHoldTime += Time.deltaTime;
            if (deleteButtonHoldTime >= deleteButtonHoldThreshold)
            {
                ClearAllTiles();
                deleteButtonHoldTime = 0f;
            }
        }

        if (loadButton != null && Input.GetMouseButton(0))
        {
            loadButtonHoldTime += Time.deltaTime;
            if (loadButtonHoldTime >= loadButtonHoldThreshold)
            {
                DeleteAllSavedMaps();
                loadButtonHoldTime = 0f;
            }
        }
        else
        {
            loadButtonHoldTime = 0f;
        }

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 gridPos = CalculateHexagonPosition(mousePos);

            if (Input.GetMouseButtonDown(0))
            {
                if (isDeleteMode)
                {
                    Collider2D hitCollider = Physics2D.OverlapPoint(gridPos, tileLayerMask);
                    if (hitCollider != null)
                    {
                        placedTiles.Remove(hitCollider.gameObject);
                        Destroy(hitCollider.gameObject);
                        Debug.Log("Tile removed at " + gridPos);
                    }
                    else
                    {
                        Debug.Log("No tile to remove at " + gridPos);
                    }
                }
                else if (currentSprite != null)
                {
                    Collider2D hitCollider = Physics2D.OverlapPoint(gridPos, tileLayerMask);
                    if (hitCollider == null)
                    {
                        GameObject newTile = Instantiate(tilePrefab, gridPos, Quaternion.identity);
                        newTile.GetComponent<SpriteRenderer>().sprite = currentSprite;
                        newTile.layer = LayerMask.NameToLayer("Tile");
                        newTile.name = currentSprite.name;  // Assign the sprite name to the tile
                        placedTiles.Add(newTile);
                        Debug.Log("Tile placed at " + gridPos);
                    }
                    else
                    {
                        Debug.Log("Tile already exists at " + gridPos);
                    }
                }
            }
        }
    }

    void HandleCameraMovement()
    {
        float moveX = Input.GetAxis("Horizontal") * cameraSpeed * Time.deltaTime;
        float moveY = Input.GetAxis("Vertical") * cameraSpeed * Time.deltaTime;

        Camera.main.transform.position += new Vector3(moveX, moveY, 0);
    }

    Vector3 CalculateHexagonPosition(Vector3 mousePos)
    {
        float hexWidth = 1f;
        float hexHeight = Mathf.Sqrt(3) / 2f;
        float offsetX = 0.75f * hexWidth;
        float offsetY = hexHeight;

        int col = Mathf.RoundToInt(mousePos.x / offsetX);
        int row = Mathf.RoundToInt((mousePos.y - (col % 2) * hexHeight / 2f) / offsetY);

        float xPos = col * offsetX;
        float yPos = row * offsetY + (col % 2) * hexHeight / 2f;

        return new Vector3(xPos, yPos, 0);
    }

    void SaveMap(string mapName)
    {
        List<TileData> tileDataList = new List<TileData>();
        foreach (GameObject tile in placedTiles)
        {
            SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
            string spriteName = renderer.sprite.name;
            TileData data = new TileData(new SerializableVector3(tile.transform.position), spriteName);
            tileDataList.Add(data);
            Debug.Log($"Saving tile at {data.position.ToVector3()} with sprite {data.spriteName}");
        }
        MapSaveLoad.SaveMap(mapName, tileDataList);
        Debug.Log("Map saved!");
    }

    void LoadMap(string mapName)
    {
        ClearAllTiles();

        // Charger les nouvelles tuiles
        List<TileData> tileDataList = MapSaveLoad.LoadMap(mapName);
        foreach (TileData data in tileDataList)
        {
            Debug.Log($"Loading tile at {data.position.ToVector3()} with sprite {data.spriteName}");
            GameObject newTile = Instantiate(tilePrefab, data.position.ToVector3(), Quaternion.identity);
            Sprite sprite = GetSpriteByName(data.spriteName);
            if (sprite != null)
            {
                newTile.GetComponent<SpriteRenderer>().sprite = sprite;
                newTile.layer = LayerMask.NameToLayer("Tile");
                newTile.name = sprite.name; // Assign the sprite name to the tile
                placedTiles.Add(newTile);
                Debug.Log($"Tile placed at {data.position.ToVector3()} with sprite {data.spriteName}");
            }
            else
            {
                Debug.LogError($"Sprite {data.spriteName} not found!");
            }
        }
        Debug.Log("Map loaded!");
    }

    void PopulateLoadDropdown()
    {
        loadDropdown.ClearOptions();
        List<string> mapNames = MapSaveLoad.GetSavedMaps();
        loadDropdown.AddOptions(mapNames);
    }

    void ClearAllTiles()
    {
        Debug.Log("Clearing all tiles...");
        foreach (GameObject tile in placedTiles)
        {
            Destroy(tile);
        }
        placedTiles.Clear();
        Debug.Log("All tiles cleared.");
    }

    void DeleteAllSavedMaps()
    {
        Debug.Log("Deleting all saved maps...");
        MapSaveLoad.DeleteAllMaps();
        PopulateLoadDropdown();
        Debug.Log("All saved maps deleted.");
    }

    Sprite GetSpriteByName(string spriteName)
    {
        
        switch (spriteName)
        {
            
            case "HexTile_Sheet_1A_5":
                return zone1Sprite;
            case "HexTile_Sheet_1A_10":
                return zone2Sprite;
            case "HexTile_Sheet_1A_25":
                return zone3Sprite;
            case "ExtraProps_83":
                return bossSprite;
            case "ExtraProps_10e":
                return shopSprite;
            default:
                // Try to load the sprite dynamically from the Resources folder
                Sprite sprite = Resources.Load<Sprite>(spriteName);
                if (sprite == null)
                {
                    Debug.LogError($"Sprite o{spriteName}o not found!");
                    
                }
                return sprite;
        }
    }
}
