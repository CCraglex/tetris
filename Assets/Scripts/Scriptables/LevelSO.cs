using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Level")]

public class LevelSO : ScriptableObject
{
    public int levelWidth;
    public int levelHeight;

    [FormerlySerializedAs("TimePerStep")]
    public float StepsPerSecond;
    
    public List<LevelTile> Tiles;
    public TetrisBlockSO TetrisBlockData;
    
    private const int LevelMinWidth = 6;

    private TileBase GetTile(LevelTilepackSO currentPack,string tileName)
    {
        var entry = Tiles.FirstOrDefault(tile => tile.tileToUse == tileName);
        if (entry == null)
            throw new Exception($"Tile '{tileName}' not found in LevelSO");

        switch (entry.tileToUse)
        {
            case "Wall":
                return currentPack.Wall;
            case "Flag":
                return currentPack.Flag;

            default:
                return null;
        }
    }

    public Vector3Int[] GetSpots(string tileName)
    {
        return Tiles
            .Where(tile => tile.tileToUse == tileName)
            .Select(tile => tile.position)
            .ToArray();
    }

    public int GetWidth()
        => levelWidth > LevelMinWidth ? levelWidth : LevelMinWidth;
    public int GetHeight()
        => levelHeight;
    public async Task PlaceTiles()
    {
        LevelTilepackSO Tilepack = await AssetLoader.LoadTilepackSO();
        Tilemap tilemap = FindAnyObjectByType<Tilemap>();

        await Task.Delay(10);

        var wallTile = GetTile(Tilepack,"Wall");
        var wallSpots = GetSpots("Wall");
        var wallTiles = wallSpots.Select(_ => wallTile).ToArray();

        foreach (var item in wallSpots)
            tilemap.SetTile(item,wallTile);
        


        var flagTile = GetTile(Tilepack,"Flag");
        var flagSpots = GetSpots("Flag");
        foreach (var item in flagSpots)
            tilemap.SetTile(item,flagTile);

        var coinSpots = GetSpots("Coin");
        GameObject.FindAnyObjectByType<CoinHandler>().SpawnCoins(coinSpots);

        BoundsInt cellBounds = tilemap.cellBounds;

        // Size in world units
        Vector3 worldSize = Vector3.Scale(
            cellBounds.size,
            tilemap.layoutGrid.cellSize
        );

        levelHeight = Mathf.CeilToInt(worldSize.y);
        levelWidth = Mathf.CeilToInt(worldSize.x);
    }

    public Vector2[] GetPlayerTiles(out Vector2 pivotPoint)
    {
        pivotPoint = TetrisBlockData.pivot;
        var spot = GetSpots("PlayerRoot")[0];

        Vector2 root = new(spot.x,spot.y);
        Vector2 spawnPoint = root + pivotPoint;
        var localTiles = TetrisBlockData.localPositions;

        Vector2[] retVal = new Vector2[localTiles.Length];
        for (int i = 0; i < localTiles.Length; i++)
            retVal[i] = spawnPoint - pivotPoint + localTiles[i];

        return retVal;
    }
}

[System.Serializable]
public class LevelTile
{
    public Vector3Int position;
    public string tileToUse;
}