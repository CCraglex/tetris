using System.Collections;
using GooglePlayGames;
using UnityEngine;

public class SaveInitializer : MonoBehaviour
{
    public IEnumerator LoadSave()
    {
        bool done = false;

        SaveStateHandler.Load(() => done = true);
        while (!done)
            yield return null;
        Debug.Log("Save ready, game starts");
    }
}