#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelSO))]
public class LevelSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelSO level = (LevelSO)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Calculated Stats", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Tile Count", level.Tiles.Count.ToString());

        float totalSeconds = level.TimePerStep * level.levelHeight;
        TimeSpan time = TimeSpan.FromSeconds(totalSeconds);

        EditorGUILayout.LabelField(
            "Estimated Time",
            time.ToString(@"m\:ss")
        );
        
        float timeFactor = Mathf.Sqrt(totalSeconds);

        float sizeFactor = 1f + (level.levelWidth * 0.05f);

        float speedRatio = 1f / level.TimePerStep;
        float speedFactor = Mathf.Pow(2f, speedRatio);

        float difficulty =
            timeFactor *
            sizeFactor *
            speedFactor;

        EditorGUILayout.LabelField("Difficulty", difficulty.ToString("0.00"));
    }
}
#endif