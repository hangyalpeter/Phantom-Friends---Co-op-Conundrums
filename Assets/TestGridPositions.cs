using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// TODO: remove class
public class TestGridPositions : MonoBehaviour
{
    [SerializeField]
    private Tilemap tilemap; // Reference to your tilemap component
    // Start is called before the first frame update
    void Start()
    {
        GetPositions();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetPositions()
    {
        // Get the bounds of the tilemap
        BoundsInt bounds = tilemap.cellBounds;

        // Loop through each cell within the bounds
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);

                // Check if there's a tile at this position before accessing it
                if (tilemap.HasTile(cellPosition))
                {
                    Vector3 worldPosition = tilemap.GetCellCenterLocal(cellPosition);
                    Debug.Log("World position of cell (" + x + ", " + y + "): " + worldPosition);
                }
            }
        }
    }
}
