using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapLoader : MonoBehaviour
{
    public Tilemap tilemap;

    void Start()
    {
        string selectedLevel = PlayerPrefs.GetString("SelectedLevel", string.Empty);
        if (!string.IsNullOrEmpty(selectedLevel))
        {
            LoadMap(selectedLevel);
        }
        else
        {
            Debug.LogError("No level selected!");
        }
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
                Debug.Log($"Tile placed at {tilePosition} with tile {data.spriteName}");
            }
            else
            {
                Debug.LogError($"Tile {data.spriteName} not found!");
            }
        }
        Debug.Log("Map loaded!");
    }

    Tile GetTileByName(string tileName)
    {
       
        Tile tile = Resources.Load<Tile>(tileName);
        if (tile == null)
        {
            Debug.LogError($"Tile {tileName} not found in Resources!");
        }
        return tile;
    }
}
