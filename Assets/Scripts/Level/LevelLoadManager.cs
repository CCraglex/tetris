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
        await levelData.PlaceTiles(Map);

        collisionHandler.SetupData(Player,levelData);
        cameraHandler.UpdateCameraStats(levelData);
        levelGameplay.lastLoadedLevel = levelID;
        collisionHandler.UpdateCoins(coinHandler.SpawnCoins(Map));
    }

    public void ReadyLevel(int levelID)
        => levelGameplay.StartCoroutine(levelGameplay.StartPlayingLevel(levelID));
    
    public void ClearMap()
        => Map.ClearAllTiles();
}
