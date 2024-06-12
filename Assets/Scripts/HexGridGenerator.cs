using UnityEngine;

public class HexGridGenerator : MonoBehaviour
{
    public GameObject hexPrefab;
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float hexWidth = 1.732f;
    public float hexHeight = 2.0f;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                float xPos = x * hexWidth * 0.75f;
                if (y % 2 == 1)
                {
                    xPos += hexWidth * 0.5f;
                }
                float yPos = y * (hexHeight * 0.75f);
                GameObject hex = Instantiate(hexPrefab, new Vector3(xPos, yPos, 0), Quaternion.identity);
                hex.transform.SetParent(this.transform);
            }
        }
    }
}
