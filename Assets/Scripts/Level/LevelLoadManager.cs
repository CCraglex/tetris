using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelLoadManager : MonoBehaviour
{
    [SerializeField] Tilemap Map;
    [SerializeField] Player Player;
    
    [SerializeField] LevelCamera cameraHandler;
    [SerializeField] LevelGameplay levelGameplay;
    [SerializeField] LevelCollisionHandler collisionHandler;
    [SerializeField] CoinHandler coinHandler;
    private LevelSO levelData;

    public async Task CreateLevel(int levelID)
    {   
        levelGameplay.BlockPausing();
        levelData = await AssetLoader.LoadLevel(levelID);
        Vector3Int[] coinSpots = levelData.GetSpots("Coin");
        collisionHandler.UpdateCoins(coinSpots);
        await Task.Yield();
        await levelData.PlaceTiles();
        collisionHandler.SetupData(Player,levelData);
        cameraHandler.UpdateCameraStats(levelData);
        levelGameplay.lastLoadedLevel = levelID;
    }

    public void ReadyLevel(int levelID)
        => levelGameplay.StartPlayingLevel(levelID);
    
    public void ClearMap()
    {
        Map.ClearAllTiles();
        coinHandler.ClearCoins();
    }
}
