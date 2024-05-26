using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap;

    [SerializeField]
    private Tilemap wallTilemap;

    [SerializeField]
    private Tile floorTile;

    [SerializeField]
    private Tile corridorTile;

    [SerializeField]
    private Tile doorTile;

    [SerializeField]
    private Tile wallTop;

    public Tilemap FloorTilemap => floorTilemap;
    public Tilemap WallTilemap => wallTilemap;
    void Start()
    {

    }

    void Update()
    {
        
    }
    public void PaintFloorTiles(IEnumerable<Vector3Int> floorPositions, UnityEngine.Color? color)
    {
        PaintTiles(floorPositions, floorTilemap, floorTile, color);
    }
    public void PaintCorridorTiles(IEnumerable<Vector3Int> floorPositions, Color? color)
    {
        PaintTiles(floorPositions, floorTilemap, corridorTile, color);
    }

    private void PaintTiles(IEnumerable<Vector3Int> positions, Tilemap tilemap, Tile tile, Color? color)
    {
        foreach (var position in positions)
        {
            //var tilePosition2 = tilemap.WorldToCell((Vector3Int)position);
            PaintSingleTile(tilemap, tile, position, color);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, Tile tile, Vector3Int position, Color? color)
    {

        var tilePosition2 = tilemap.WorldToCell(Vector3Int.RoundToInt(position));
        //var tilePosition2 = position;
        //tilemap.SetTile(tilePosition, tile);

        //tilemap.SetTile(Vector3Int.RoundToInt(position), tile);
        tilemap.SetTile(tilePosition2, tile);
        tilemap.SetTileFlags(tilePosition2, TileFlags.None);
        if (color != null)
        {
            tilemap.SetColor(tilePosition2, (Color)color);
        }
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

    internal void PaintDoorTile(Vector3Int position, Color color)
    {
        PaintSingleTile(floorTilemap, doorTile, position, color);
    }

 }
