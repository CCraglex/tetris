using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class LevelHandler
{
    public static Tilemap Map;
    public static Player Player;
    public static LevelTextHandler levelTextHandler;

    public static ImmunityPowerup powerupInstance;
    public static CrushedTileGenerator tileAnimator;

    public static bool Immune;
    public static bool AllowedToPowerup;

    public static List<Vector2Int> playerTiles;
    private static List<Vector2Int> wallGridPositions;
    private static List<Vector2Int> flagGridPositions;

    private static LevelCamera cameraHandler;
    
    //3 ticks of 0.45 sec
    public const int IntroTickTimer = 450;

    public static List<Vector2Int> GetWalls()
        => wallGridPositions;
    public static List<Vector2Int> GetFlags()
        => flagGridPositions;


    private static void MapToGrids(LevelSO level)
    {
        wallGridPositions = level.Tiles
            .Where(tile => tile.tileToUse == "Wall")
            .Select(tile => new Vector2Int(tile.position.x,tile.position.y))
            .ToList();
        
        flagGridPositions = level.Tiles
            .Where(tile => tile.tileToUse == "Flag")
            .Select(tile => new Vector2Int(tile.position.x,tile.position.y))
            .ToList();
    }

    public static void InitLevel(LevelSO level)
    {
        if(Player == null)
            Player = GameObject.FindFirstObjectByType<Player>();
        if(cameraHandler == null)
            cameraHandler = Camera.main.GetComponent<LevelCamera>();
        
        MapToGrids(level);
        Player.GeneratePlayer();
        cameraHandler.UpdateCameraStats(level.GetWidth(),level.GetHeight());
        CoinHandler.GetCoinHandler().SpawnCoins(level.GetWidth() * level.GetHeight() / 20,Map);
    }

    public static async Task IntroLevel()
    {
        await levelTextHandler.PlayBeginning();
    }

    public static void StartPlaying(int levelIndex)
    {
        levelTextHandler.SetLevel(levelIndex.ToString());
        levelTextHandler.StartCoroutine(levelTextHandler.PlayAnim());

        Player.StartPlaying();
        cameraHandler.StartAction();
    }

    private static void CrushTiles(List<Vector2Int> Tiles)
    {
        foreach (var item in Tiles)
        {
            wallGridPositions.Remove(item);
            tileAnimator.CreateFakeTileAt(Map,new(item.x,item.y));
        }
    }

    public static bool CanLandHere()
    {
        Vector2Int playerPos = new(
            Mathf.RoundToInt(Player.transform.position.x),
            Mathf.RoundToInt(Player.transform.position.y)
        );

        var hittingTiles = new List<Vector2Int>();

        foreach (var playerTile in playerTiles)
        {
            Vector2Int lowerTile = playerPos + new Vector2Int(playerTile.x, playerTile.y - 1);

            if (wallGridPositions.Contains(lowerTile))
                hittingTiles.Add(lowerTile);
        }

        if(hittingTiles.Count > 0 && Immune)
        {
            powerupInstance.SetImmunity(false);
            CrushTiles(hittingTiles);
        }

        return hittingTiles.Count > 0 && Immune == false;
    }


    public static bool HasLandedOnFlag()
    {
        var combinedTiles = new List<Vector2Int>(flagGridPositions);
        combinedTiles.AddRange(playerTiles);

        Vector2Int playerPos = new(
            Mathf.RoundToInt(Player.transform.position.x),
            Mathf.RoundToInt(Player.transform.position.y)
        );

        foreach (var playerTile in playerTiles)
        {
            if (combinedTiles.Contains(new(Mathf.RoundToInt(playerPos.x + playerTile.x), Mathf.RoundToInt(playerPos.y + playerTile.y))))
                return true;
        }
        return false;
    }

}
