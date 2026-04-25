using System;
using System.Threading.Tasks;
using GoogleMobileAds.Api;
using UnityEngine;

public class InterstitialAdHandler : IAd
{
    public InterstitialAd LoadedAd { get; private set; }
    public string ID { get; set; }

    public Action OnClosedAction;
    public Action OnFailedAction;

    private bool isLoadingAd;

    public void LoadAd(int attempt = 0)
    {
        if (!AdService.adsInitialized || isLoadingAd)
            return;

        isLoadingAd = true;

        var request = new AdRequest();

        InterstitialAd.Load(ID, request, (ad, error) =>
        {
            MainThreadDispatcher.Run(() =>
            {
                isLoadingAd = false;

                if (error != null || ad == null)
                {
                    _ = Retry(() => LoadAd(attempt + 1), attempt);
                    return;
                }

                LoadedAd = ad;

                LoadedAd.OnAdFullScreenContentClosed += OnClosed;
                LoadedAd.OnAdFullScreenContentFailed += OnFailed;
            });
        });
    }

    private async Task Retry(Action retryAction, int attempt)
    {
        if (LoadedAd != null)
            return;

        if (attempt >= 5)
        {
            MainThreadDispatcher.Run(() =>
            {
                Debug.LogWarning("Ad load failed after max retries.");
            });
            return;
        }

        float delaySeconds = Mathf.Pow(2, attempt);
        int delayMs = (int)(delaySeconds * 1000);

        await Task.Delay(delayMs);

        MainThreadDispatcher.Run(() =>
        {
            retryAction();
        });
    }

    public void Show()
    {
        if (LoadedAd != null && LoadedAd.CanShowAd())
        {
            AdService.EnterAdMode();
            LoadedAd.Show();
        }
        else
        {
            OnFailedAction?.Invoke();
            LoadAd();
        }
    }

    public void OnClosed()
    {
        MainThreadDispatcher.Run(() =>
        {
            OnClosedAction?.Invoke();
            Cleanup();
        });
    }

    public void OnFailed(AdError error)
    {
        MainThreadDispatcher.Run(() =>
        {
            OnFailedAction?.Invoke();
            Cleanup();
        });
    }

    public void Cleanup()
    {
        if (LoadedAd != null)
        {
            LoadedAd.OnAdFullScreenContentClosed -= OnClosed;
            LoadedAd.OnAdFullScreenContentFailed -= OnFailed;

            LoadedAd.Destroy();
            LoadedAd = null;
        }

        LoadAd();
    }
}