
using UnityEngine;

public class SaveAppliactionEvents : MonoBehaviour
{
    void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveStateHandler.Save();
    }

    void OnApplicationQuit()
        => SaveStateHandler.SaveToCloud();
}