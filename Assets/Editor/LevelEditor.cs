
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using System.IO;


[CustomEditor(typeof(LevelEngine))]
public class LevelEditor : Editor
{
    private int customID;
    private int levelID;
    private int Difficulty = 1;

    private List<Vector3Int> GetSpots(Tilemap Tiles,string tileName)
    {
        var level = target as LevelEngine;
        return level.GetSpotsOfTiles(tileName);
    }

    private void HandleTile(LevelSO Level,string Tile)
    {
        var level = target as LevelEngine;
        var map = level.GetTilemap();
        List<Vector3Int> Spots = GetSpots(map, Tile);
        Level.Tiles ??= new();

        foreach (var Spot in Spots)
            Level.Tiles.Add(new(){tileToUse = Tile,position = Spot});        
    }

    private void CreateNewLevelAsset(int ID)
    {
        if(Difficulty == 0)
            throw new InvalidDataException("Difficulty value can't be 0!");
        if(ID < 1)
            throw new InvalidDataException("Level ID can't be lower than 1!");
        LevelSO newLevel = CreateInstance<LevelSO>();
        newLevel.TimePerStep = Difficulty;
        
        var level = target as LevelEngine;
        var map = level.GetTilemap();

        HandleTile(newLevel,"Wall");
        HandleTile(newLevel,"Flag");
        HandleTile(newLevel,"Player");
        HandleTile(newLevel,"PlayerRoot");
        HandleTile(newLevel,"Coin");

        newLevel.levelWidth = map.cellBounds.size.x;
        newLevel.levelHeight = map.cellBounds.size.y;

        string path = "Assets/Addressables/Levels/Level-" + ID + ".asset";
        AssetDatabase.CreateAsset(newLevel,path);
        if(AddToAddressables(path,ID))
            Debug.Log($"Level-{ID} has been created successfully!");
        
        levelID = GetNextID();
    }

    private bool AddToAddressables(string assetPath, int id)
    {
        AddressableAssetSettings settings =
            AddressableAssetSettingsDefaultObject.Settings;

        if (settings == null)
        {
            Debug.LogError("AddressableAssetSettings not found.");
            return false;
        }

        // Find or create a group
        AddressableAssetGroup group =
            settings.FindGroup("Levels");

        if (group == null)
        {
            group = settings.CreateGroup(
                "Levels",
                false,
                false,
                false,
                new List<AddressableAssetGroupSchema>
                {
                    settings.DefaultGroup.Schemas[0],
                    settings.DefaultGroup.Schemas[1]
                });
        }

        string guid = AssetDatabase.AssetPathToGUID(assetPath);

        AddressableAssetEntry entry =
            settings.CreateOrMoveEntry(guid, group);

        // Set address
        entry.address = $"Level-{id}";

        // Optional but very useful
        entry.SetLabel("Level", true);

        // Save
        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
        AssetDatabase.SaveAssets();

        return true;
    }

    private int GetNextID()
    {
        string folderPath = "Assets/Addressables/Levels";

        string[] guids = AssetDatabase.FindAssets(
            "t:LevelSO",
            new[] { folderPath }
        );

        return guids.Length + 1;
    }

    void OnEnable()
    {
        levelID = GetNextID();
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Create new level:");
        EditorGUILayout.LabelField("Current Last Index: " + levelID);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Difficulty: (How many steps per second)");
        Difficulty = EditorGUILayout.IntField(Difficulty);
        EditorGUILayout.EndHorizontal();

        if(GUILayout.Button("Update index"))
            levelID = GetNextID();

        if(GUILayout.Button("Create Level"))
            CreateNewLevelAsset(GetNextID());
        
        EditorGUILayout.LabelField("Edit existing level:");

        EditorGUILayout.BeginHorizontal();
        customID = EditorGUILayout.IntField($"Level To Edit: {customID}",customID);
        if(GUILayout.Button("Edit Level"))
            CreateNewLevelAsset(customID);
        if(GUILayout.Button("Clear Level"))
        {
            var level = target as LevelEngine;
            Tilemap t = level.GetComponent<Tilemap>();
            t.ClearAllTiles();
            t.CompressBounds();
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}

#endif