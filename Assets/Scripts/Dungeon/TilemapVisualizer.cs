using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap;

    [SerializeField]
    private Tilemap wallTilemap;

    [SerializeField]
    private TileBase floorTile;

    [SerializeField]
    private TileBase corridorTile;

    [SerializeField]
    private TileBase doorTile;

    [SerializeField]
    private TileBase wallTop;

    public Tilemap FloorTilemap => floorTilemap;
    public Tilemap WallTilemap => wallTilemap;
    public void PaintFloorTiles(IEnumerable<Vector3Int> floorPositions, Color? color)
    {
        PaintTiles(floorPositions, floorTilemap, floorTile, color);
    }
    public void PaintCorridorTiles(IEnumerable<Vector3Int> floorPositions, Color? color)
    {
        PaintTiles(floorPositions, floorTilemap, corridorTile, color);
    }

    private void PaintTiles(IEnumerable<Vector3Int> positions, Tilemap tilemap, TileBase tile, Color? color)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tile, position, color);
        }
    }
    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector3Int position, Color? color)
    {

        var tilePosition = tilemap.WorldToCell(Vector3Int.RoundToInt(position));

        var tilePosition2 = position;

        tilemap.SetTile(tilePosition, tile);
        if (color != null)
        {
            tilemap.SetTileFlags(tilePosition, TileFlags.None);
            tilemap.SetColor(tilePosition, (Color)color);
        }
    }

    private void ClearSingleTile(Tilemap tilemap, Vector3Int position)
    {
        var tilePosition = tilemap.WorldToCell(Vector3Int.RoundToInt(position));
        tilemap.SetTile(tilePosition, null);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }
    internal void PaintSingleWall(Vector3Int wallPosition, Color? color)
    {
        PaintSingleTile(wallTilemap, wallTop, wallPosition, color);
    }
    internal void PaintDoorTile(Vector3Int position, Color? color)
    {
        PaintSingleTile(wallTilemap, doorTile, position, color);
    }
    internal void PaintOpenGateTile(Vector3Int position, Color? color)
    {
        ClearSingleTile(wallTilemap, position);
    }


}
