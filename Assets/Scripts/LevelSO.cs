using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Level")]

public class LevelSO : ScriptableObject
{
    public int levelWidth;
    public int levelHeight;

    public float TimePerStep;
    public List<LevelTile> Tiles;

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
    public async Task PlaceTiles(Tilemap tilemap)
    {
        LevelTilepackSO Tilepack = await AssetLoader.LoadTilepackSO();

        var wallTile = GetTile(Tilepack,"Wall");
        var wallSpots = GetSpots("Wall");
        tilemap.SetTiles(
            wallSpots,
            wallSpots.Select(_ => wallTile).ToArray()
        );

        var flagTile = GetTile(Tilepack,"Flag");
        var flagSpots = GetSpots("Flag");
        tilemap.SetTiles(
            flagSpots,
            flagSpots.Select(_ => flagTile).ToArray()
        );

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

    public Vector2Int[] GetPlayerTiles(out Vector2 spawnPos)
    {
        // --- FIX 4: enforce exactly one root
        var roots = GetSpots("PlayerRoot");
        if (roots.Length != 1)
            throw new Exception("Level must contain exactly one PlayerRoot tile");

        var root = roots[0];

        var playerSpots = GetSpots("Player");
        Vector2Int rootV2Int = new(root.x, root.y);

        Vector2Int[] retVal = new Vector2Int[playerSpots.Length + 1];
        for (int i = 0; i < playerSpots.Length; i++)
        {
            retVal[i] =
                new Vector2Int(playerSpots[i].x, playerSpots[i].y)
                - rootV2Int;
        }

        spawnPos = rootV2Int;
        retVal[playerSpots.Length] = Vector2Int.zero;

        return retVal;
    }
}

[System.Serializable]
public class LevelTile
{
    public Vector3Int position;
    public string tileToUse;
}