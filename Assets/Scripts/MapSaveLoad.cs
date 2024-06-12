using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class MapSaveLoad
{
    private static string saveDirectory = Application.persistentDataPath + "/Maps/";

    public static void SaveMap(string mapName, List<TileData> tileDataList)
    {
        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }

        string savePath = saveDirectory + mapName + ".dat";
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream fileStream = new FileStream(savePath, FileMode.Create))
        {
            formatter.Serialize(fileStream, tileDataList);
        }
    }

    public static List<TileData> LoadMap(string mapName)
    {
        string loadPath = saveDirectory + mapName + ".dat";
        if (File.Exists(loadPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fileStream = new FileStream(loadPath, FileMode.Open))
            {
                return (List<TileData>)formatter.Deserialize(fileStream);
            }
        }
        return new List<TileData>();
    }

    public static List<string> GetSavedMaps()
    {
        if (!Directory.Exists(saveDirectory))
        {
            return new List<string>();
        }

        List<string> mapNames = new List<string>();
        foreach (string filePath in Directory.GetFiles(saveDirectory, "*.dat"))
        {
            mapNames.Add(Path.GetFileNameWithoutExtension(filePath));
        }
        return mapNames;
    }

    public static void DeleteAllMaps()
    {
        if (Directory.Exists(saveDirectory))
        {
            foreach (string filePath in Directory.GetFiles(saveDirectory, "*.dat"))
            {
                File.Delete(filePath);
            }
        }
    }
}
