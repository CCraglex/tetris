
using System.IO;
using UnityEngine;

public static class SaveStateHandler
{
    private static SaveData currentdata;
    const string Path = "/SaveData.json";

    public static void Load()
    {
        var fullPath = Application.persistentDataPath + Path;
        if (File.Exists(fullPath))
        {
            string data = File.ReadAllText(fullPath);
            if(data != string.Empty)
                currentdata = JsonUtility.FromJson<SaveData>(data);
        }
            
    
        else currentdata = new()
        {
            cash = 10,
            currentLevel = 1,
            MaxLevel = 1,
            upg1Amount = 3,
            upg2Amount = 3,
            upg3Amount = 3  
        };
    }

    public static void Save()
    {
        string parsedToJson = JsonUtility.ToJson(currentdata);
        File.WriteAllText(Application.persistentDataPath + Path,parsedToJson);
    } 

    public static int GetCurrentLevel()
        => currentdata.currentLevel;
    
    public static int GetNextLevel()
        => currentdata.currentLevel + 1;
    
    public static int GetPreviousLevel()
        => currentdata.currentLevel - 1;
    
    public static void SetLevel(int newLevel)
    {
        currentdata.currentLevel = newLevel;
        Save();
    }
    public static void FinishLevel(){
        currentdata.MaxLevel += 1;
        Save();
    }
    

    public static void AddCash(int amount){
        currentdata.cash += amount;
        Save();
    }
    
    public static void RemoveCash(int amount){
        currentdata.cash -= amount;
        Save();
    }
    public static bool HasCash(int amount)
        => currentdata.cash >= amount;
}

[System.Serializable]
public class SaveData
{
    public int cash;
    public int currentLevel;
    public int MaxLevel;

    public int upg1Amount;
    public int upg2Amount;
    public int upg3Amount;
}