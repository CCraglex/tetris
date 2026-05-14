using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System.Threading.Tasks;

[System.Serializable]
public class SaveData
{
    public int version;

    public int cash;
    public int MaxLevel;
    public int upg1Amount;

    public long lastSaveTime;

    public string hash;
}

public static class SaveStateHandler
{
    public static Action<int> CashChanged;
    public static Action<int> PowerupChanged;

    public static SaveData currentdata;

    const string FILE_NAME = "/SaveData.json";
    const string CLOUD_KEY = "save_game";
    const int CURRENT_VERSION = 1;

    private const string SECRET = "a7f3K9x2QmP1"; // anti-tamper key
    private static long LastCloudSave;
    private static bool IsDirty;

    // ---------------- LOCAL + CLOUD LOAD ----------------
    public static void Load(Action onComplete)
    {
        Debug.Log("Loading...");
        bool forceLocal = true;
        if (forceLocal)
        {
            ForceLocalLoad();
            onComplete.Invoke();           
            return; 
        }

        ISavedGameClient client = PlayGamesPlatform.Instance.SavedGame;
        client.OpenWithAutomaticConflictResolution(
            CLOUD_KEY,
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            (status, game) =>
            {
                if (status != SavedGameRequestStatus.Success || game == null)
                {
                    Debug.Log("No cloud save");
                    ForceLocalLoad();
                    onComplete.Invoke();
                    return;
                }

                client.ReadBinaryData(game, (readStatus, data) =>
                {
                    if (readStatus != SavedGameRequestStatus.Success)
                    {
                        Debug.Log("Cloud read failed");
                        ForceLocalLoad();
                        onComplete.Invoke();
                        return;
                    }

                    string text = System.Text.Encoding.UTF8.GetString(data);

                    // ✅ DELETE SYNC CHECK
                    if (text == "deleted")
                    {
                        Debug.Log("Cloud is deleted → resetting");

                        currentdata = CreateNewSave();
                        Save();
                        onComplete.Invoke();
                        return;
                    }

                    // ✅ NORMAL SAVE
                    SaveData save = JsonUtility.FromJson<SaveData>(text);

                    if (save != null && IsValid(save))
                    {
                        currentdata = save;
                        onComplete.Invoke();
                        Debug.Log("Cloud loaded");
                    }
                    else
                    {
                        Debug.LogWarning("Invalid cloud save → falling back to local");
                        ForceLocalLoad();
                        onComplete.Invoke();
                    }
                });
            });
    }
    private static void ForceLocalLoad()
    {
        string path = Application.persistentDataPath + FILE_NAME;

        Debug.Log("Loading local: " + path);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            if (!string.IsNullOrEmpty(json))
            {
                var local = JsonUtility.FromJson<SaveData>(json);

                if (local != null)
                {
                    currentdata = local;
                    Debug.Log("Force loaded local save");
                    return;
                }
            }
        }

        Debug.LogWarning("Creating new save");
        currentdata = CreateNewSave();
    }
    // ---------------- CREATE NEW SAVE ----------------
    public static SaveData CreateNewSave()
    {
        return new SaveData
        {
            version = CURRENT_VERSION,
            cash = 10,
            MaxLevel = 0,
            upg1Amount = 3,
            lastSaveTime = Now(),
            hash = ""
        };
    }

    private static long Now()
        => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    // ---------------- LOCAL SAVE ----------------
    public static void Save(bool forceCloud = false)
    {
        IsDirty = true;
        currentdata.lastSaveTime = Now();
        currentdata.hash = GenerateHash(currentdata);

        string json = JsonUtility.ToJson(currentdata);
        File.WriteAllText(Application.persistentDataPath + FILE_NAME, json);

        if(forceCloud || IsDirty && currentdata.lastSaveTime > LastCloudSave + 60)
        {
            SaveToCloud();
            IsDirty = false;
            LastCloudSave = currentdata.lastSaveTime;
        }
    }

    // ---------------- VERSIONING ----------------
    private static void MigrateIfNeeded(SaveData data)
    {
        if (data.version < CURRENT_VERSION)
        {
            data.version = CURRENT_VERSION;
        }
    }

    // ---------------- HASH (ANTI-TAMPER) ----------------
    private static string GenerateHash(SaveData data)
    {
        string raw =
            data.version +
            data.cash +
            data.MaxLevel +
            data.upg1Amount +
            data.lastSaveTime +
            SECRET;

        using (SHA256 sha = SHA256.Create())
        {
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));

            StringBuilder sb = new StringBuilder();
            foreach (var b in bytes)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }
    }

    private static bool IsValid(SaveData data)
    {
        if (data == null) return false;
        return GenerateHash(data) == data.hash;
    }

    // ---------------- CLOUD SAVE ----------------
    public static void SaveToCloud(Action onComplete = null)
    {
        string json = JsonUtility.ToJson(currentdata);
        byte[] bytes = Encoding.UTF8.GetBytes(json);

        ISavedGameClient client = PlayGamesPlatform.Instance.SavedGame;
        if(client == null)
            return;
            
        client.OpenWithAutomaticConflictResolution(
            CLOUD_KEY,
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            (status, game) =>
            {
                if (status != SavedGameRequestStatus.Success) return;

                var update = new SavedGameMetadataUpdate.Builder()
                    .WithUpdatedDescription("Cloud Save")
                    .Build();

                client.CommitUpdate(game, update, bytes, (s, meta) =>
                {
                    Debug.Log("Cloud Save: " + s);
                });

                onComplete?.Invoke();
                IsDirty = false;
            });
    }

    public static void DeleteAll(Action onComplete = null)
    {
        string text = "deleted";
        byte[] bytes = Encoding.UTF8.GetBytes(text);

        ISavedGameClient client = PlayGamesPlatform.Instance.SavedGame;

        client.OpenWithAutomaticConflictResolution(
            CLOUD_KEY,
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            (status, game) =>
            {
                if (status != SavedGameRequestStatus.Success || game == null)
                {
                    Debug.LogWarning("Cloud delete failed to open save");
                    onComplete?.Invoke();
                    return;
                }

                var update = new SavedGameMetadataUpdate.Builder()
                    .WithUpdatedDescription("DELETED")
                    .Build();

                client.CommitUpdate(game, update, bytes, (s, meta) =>
                {
                    Debug.Log("Cloud marked as deleted: " + s);
                    onComplete?.Invoke();
                });
            });
    }

    // ---------------- SAFE MERGE ----------------
    private static SaveData ResolveConflict(SaveData local, SaveData cloud)
    {
        if (local == null) return cloud;
        if (cloud == null) return local;

        // pick the most recently saved whole state
        return cloud.lastSaveTime >= local.lastSaveTime ? cloud : local;
    }

    // ---------------- GAME API ----------------
    public static int GetCash() => currentdata.cash;
    public static int GetMaxLevel() => currentdata.MaxLevel;
    public static int GetPowerupCount() => currentdata.upg1Amount;

    public static void AddCash(int amount)
    {
        currentdata.cash += amount;
        CashChanged?.Invoke(currentdata.cash);
        Save();
    }

    public static void MakePurchase(int amount)
    {
        currentdata.cash += amount;
        CashChanged?.Invoke(currentdata.cash);
        Save(true);
    }

    public static void RemoveCash(int amount)
    {
        currentdata.cash -= amount;
        CashChanged?.Invoke(currentdata.cash);
        Save();
    }

    public static void FinishLevel(int level)
    {
        currentdata.MaxLevel = Math.Max(currentdata.MaxLevel, level);
        Save(true);
    }

    public static void AddPowerup(int amount)
    {
        currentdata.upg1Amount += amount;
        PowerupChanged?.Invoke(currentdata.upg1Amount);
        Save();
    }

    public static void UsePowerup()
    {
        currentdata.upg1Amount -= 1;
        PowerupChanged?.Invoke(currentdata.upg1Amount);
        Save();
    }
}