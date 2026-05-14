using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class GPGSAuth : MonoBehaviour
{
    public bool authActionCompleted;
    public GameObject loginButton;
    public void Auth() {
        StartCoroutine(ITimeout());
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status) {
        if (status == SignInStatus.Success) {
            print(status);
            loginButton.SetActive(false);
        } else {
            print(status);
        }

        authActionCompleted = true;
    }

    private IEnumerator ITimeout()
    {
        yield return new WaitForSeconds(3f);
        authActionCompleted = true;
    }

    public void LoginManual()
    {
        PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
    }

    private void OnApplicationPause(bool pause)
    {
        //We only care for loading
        if(authActionCompleted == false)
            Time.timeScale = pause ? 0 : 1;
    }
}