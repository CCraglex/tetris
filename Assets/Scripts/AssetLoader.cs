
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Tilemaps;

public static class AssetLoader
{
    private static LevelTilepackSO currentLevelTilepack;
    private static AsyncOperationHandle<LevelTilepackSO> levelTilepackHandle;

    private static LevelSO currentLevel;
    private static AsyncOperationHandle<LevelSO> levelHandle;

    #region Level tile packs
    private static void ReleaseLevelTilepackIfExists()
    {
        if(levelTilepackHandle.IsValid())
        {
            Addressables.Release(levelTilepackHandle);
            levelTilepackHandle = default;
        }
    }

    public static async Task<LevelTilepackSO> LoadTilepackSO()
    {
        ReleaseLevelTilepackIfExists();
        levelTilepackHandle = Addressables.LoadAssetAsync<LevelTilepackSO>("Level_Tiles_1");
        await levelTilepackHandle.Task;
        
        currentLevelTilepack = levelTilepackHandle.Result;
        return currentLevelTilepack;
    }

    public static LevelTilepackSO GetTilepackSO()
        => currentLevelTilepack;

    #endregion

    public static void UnloadLevelIfExists()
    {
        if (levelHandle.IsValid())
        {
            Addressables.Release(levelHandle);
            currentLevel = null;
        }
    }

    public static async Task<LevelSO> LoadLevel(int index)
    {
        UnloadLevelIfExists();

        var handle = Addressables.LoadAssetAsync<LevelSO>($"Level-{index}");
        await handle.Task;

        if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
            return null;

        currentLevel = handle.Result;
        return currentLevel;
    }

    public static LevelSO GetCurrentLevel()
        => currentLevel;
}