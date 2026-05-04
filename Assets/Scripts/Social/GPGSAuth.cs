using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class GPGSAuth : MonoBehaviour
{
    public GameObject loginButton;
    public void Start() {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status) {
        if (status == SignInStatus.Success) {
            print(status);
            loginButton.SetActive(false);
        } else {
            print(status);
            // Disable your integration with Play Games Services or show a login button
            // to ask users to authenticate. Clicking it should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
        }
    }

    public void LoginManual()
    {
        PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
    }
}