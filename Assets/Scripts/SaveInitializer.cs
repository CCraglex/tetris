using UnityEngine;

public class SaveInitializer : MonoBehaviour
{
    public void Awake()
        => SaveStateHandler.Load();
}