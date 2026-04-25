using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class PreBuildCleanup : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("Running pre-build cleanup...");
        PlayerPrefs.DeleteAll();
        Time.timeScale = 1f;
        AssetDatabase.SaveAssets();
        Debug.Log("Pre-build cleanup complete.");
    }
}