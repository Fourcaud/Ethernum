using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameSceneController : MonoBehaviour
{
    public Tilemap tilemap;
    public Dropdown levelDropdown;
    public Button loadLevelButton;
    public float cameraSpeed = 5f;
    public GameObject playerPrefab;

    private GameObject playerInstance;

    void Start()
    {
        ClearMap(); // Clear all tiles at the start to ensure the map is empty
        PopulateLevelDropdown();
        loadLevelButton.onClick.AddListener(OnLoadLevelButtonClick);
    }

    void Update()
    {
        HandleCameraMovement();
    }

    void ClearMap()
    {
        tilemap.ClearAllTiles();
        if (playerInstance != null)
        {
            Destroy(playerInstance);
        }
    }

    void PopulateLevelDropdown()
    {
        levelDropdown.ClearOptions();
        List<string> mapNames = MapSaveLoad.GetSavedMaps();
        levelDropdown.AddOptions(mapNames);
    }

    void OnLoadLevelButtonClick()
    {
        ClearMap(); // Ensure map is cleared before loading new level
        string selectedLevel = levelDropdown.options[levelDropdown.value].text;
        LoadMap(selectedLevel);
    }

    void LoadMap(string mapName)
    {
        List<TileData> tileDataList = MapSaveLoad.LoadMap(mapName);
        foreach (TileData data in tileDataList)
        {
            Tile tile = GetTileByName(data.spriteName);
            if (tile != null)
            {
                Vector3Int tilePosition = Vector3Int.FloorToInt(data.position.ToVector3());
                tilemap.SetTile(tilePosition, tile);

                // Create a GameObject for the tile to add a collider and assign the layer
                GameObject tileObject = new GameObject("TileCollider");
                tileObject.transform.position = tilemap.GetCellCenterWorld(tilePosition);
                tileObject.AddComponent<PolygonCollider2D>();
                tileObject.layer = LayerMask.NameToLayer("HexLayer");

                Debug.Log($"Tile placed at {tilePosition} with tile {data.spriteName}");
            }
            else
            {
                Debug.LogError($"Tile {data.spriteName} not found!");
            }
        }
        Debug.Log("Map loaded!");
        PlacePlayerOnValidTile(); // Place the player on a valid tile after loading the map
    }

    Tile GetTileByName(string tileName)
    {
        Debug.Log($"Trying to load tile: {tileName}");
        Tile tile = Resources.Load<Tile>(tileName);
        if (tile == null)
        {
            Debug.LogError($"Tile {tileName} not found in Resources!");
        }
        return tile;
    }

    void HandleCameraMovement()
    {
        float moveX = Input.GetAxis("Horizontal") * cameraSpeed * Time.deltaTime;
        float moveY = Input.GetAxis("Vertical") * cameraSpeed * Time.deltaTime;

        Camera.main.transform.position += new Vector3(moveX, moveY, 0);
    }

    void PlacePlayerOnValidTile()
    {
        if (playerInstance != null)
        {
            Destroy(playerInstance); // Destroy the previous player instance if it exists
        }

        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                Vector3 playerPosition = tilemap.GetCellCenterWorld(pos);
                playerInstance = Instantiate(playerPrefab, playerPosition, Quaternion.identity);
                playerInstance.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
                playerInstance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                break;
            }
        }
    }
}
