
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
            
        currentdata = new()
        {
            cash = 10,
            MaxLevel = 1,
            upg1Amount = 3,
        };
    }

    public static void Save()
    {
        string parsedToJson = JsonUtility.ToJson(currentdata);
        File.WriteAllText(Application.persistentDataPath + Path,parsedToJson);
    } 

    public static int GetMaxLevel()
        => currentdata.MaxLevel;
    
    public static void FinishLevel(){
        currentdata.MaxLevel += 1;
        Save();
    }

    public static int GetCash()
        => currentdata.cash;
    

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
    
    public static bool HasPowerup()
        => currentdata.upg1Amount > 0;
    
    public static void UsePowerup()
    {
        currentdata.upg1Amount -= 1;
        Save();
    }
    public static int GetPowerupCount()
        => currentdata.upg1Amount;
}

[System.Serializable]
public class SaveData
{
    public int cash;
    public int MaxLevel;
    public int upg1Amount;
}