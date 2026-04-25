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
    [SerializeField] LevelTextHandler levelTextHandler;
    
    [SerializeField] LevelCamera cameraHandler;
    [SerializeField] LevelGameplay levelGameplay;
    [SerializeField] LevelCollisionHandler collisionHandler;
    [SerializeField] CoinHandler coinHandler;
    private LevelSO levelData;

    public async Task CreateLevel(int levelID)
    {   
        levelData = await AssetLoader.LoadLevel(levelID);
        Vector3Int[] coinSpots = levelData.GetSpots("Coin");
        print(coinSpots.Length);
        collisionHandler.UpdateCoins(coinSpots);

        await Task.Yield();
        await levelData.PlaceTiles(Map);

        print("?");
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
