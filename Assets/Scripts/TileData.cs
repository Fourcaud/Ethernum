using System;
using UnityEngine;

[Serializable]
public class TileData
{
    public SerializableVector3 position;
    public string spriteName;

    public TileData(SerializableVector3 position, string spriteName)
    {
        this.position = position;
        this.spriteName = spriteName;
    }

    public TileData(Vector3 position, string spriteName)
    {
        this.position = new SerializableVector3(position);
        this.spriteName = spriteName;
    }
}
