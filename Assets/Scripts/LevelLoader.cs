
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

    public static async Task LoadLevel(int Index)
    {
        UnloadLevelIfExists();
        levelHandle = Addressables.LoadAssetAsync<LevelSO>("Level-" + Index);
        await levelHandle.Task;
        
        currentLevel = levelHandle.Result;
        await currentLevel.PlaceTiles(GameObject.FindFirstObjectByType<Tilemap>());
        
        LevelHandler.InitLevel(currentLevel);
    }

    public static LevelSO GetCurrentLevel()
        => currentLevel;
}

public static class LevelLoader
{
    public static async Task LevelLoadingTask(int level)
    {
        var levelLoad = AssetLoader.LoadLevel(level);
        await levelLoad;
    }

    public static async void PlayLevel(int level)
    {
        await LevelHandler.IntroLevel();
        LevelHandler.StartPlaying(level);
    }
}