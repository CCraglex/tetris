using System.IO;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;

public class SaveDeletionHandler : MonoBehaviour
{
    [SerializeField] CanvasGroup deletePanel;

    public void TryDeleting()
    {
        deletePanel.alpha = 1;
        deletePanel.blocksRaycasts = true;
    }

    public void Close()
    {
        deletePanel.alpha = 0;
        deletePanel.blocksRaycasts = false;
    }

    public void DeleteAllUserData()
    {
        const string FILE_NAME = "/SaveData.json";
        string path = Application.persistentDataPath + FILE_NAME;

        if (File.Exists(path))
            File.Delete(path);
        SaveStateHandler.DeleteAll(() => RestartGame());
    }

    private void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }
}
