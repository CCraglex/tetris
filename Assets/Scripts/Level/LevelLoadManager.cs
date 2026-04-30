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
    [SerializeField] PausePanel pausePanel;
    [SerializeField] LevelCollisionHandler collisionHandler;
    [SerializeField] CoinHandler coinHandler;
    private LevelSO levelData;

    private void LoadLastPage()
    {
        pausePanel.ReturnToMenu("Thank you for playing the game!\nMake sure to leave a comment and rate the game if you liked it!");
    }

    public async Task CreateLevel(int levelID)
    {   
        levelGameplay.BlockPausing();
        levelData = await AssetLoader.LoadLevel(levelID);

        if(levelData == null)
        {
            LoadLastPage();
            return;
        }
            
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
