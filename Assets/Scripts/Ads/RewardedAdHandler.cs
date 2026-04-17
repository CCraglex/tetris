using System;
using System.Threading.Tasks;
using GoogleMobileAds.Api;
using UnityEngine;

public class RewardedAdHandler : IAd
{
    public RewardedAd LoadedAd {get; set;}
    public string ID {get; set;}

    public Action OnRewardedAction;
    public Action OnClosedAction;
    public Action OnFailedAction;

    private bool isLoadingAd;

    public void LoadAd(int attempt = 0)
    {
        if (!AdService.adsInitialized || isLoadingAd)
            return;

        isLoadingAd = true;
        var request = new AdRequest();

        RewardedAd.Load(ID, request, (ad, error) =>
        {
            if (error != null || ad == null)
            {
                isLoadingAd = false;
                _ = Retry(() => LoadAd(attempt + 1), attempt);
                return;
            }

            LoadedAd = ad;
            LoadedAd.OnAdFullScreenContentClosed += OnClosed;
            LoadedAd.OnAdFullScreenContentFailed += OnFailed;
            isLoadingAd = false;
        });
    }

    private async Task Retry(Action retryAction, int attempt)
    {
        if (LoadedAd != null)
            return;

        if (attempt >= 5)
        {
            await Awaitable.MainThreadAsync();
            Debug.LogWarning("Ad load failed after max retries.");
            return;
        }

        float delaySeconds = Mathf.Pow(2, attempt);
        int delayMs = (int)(delaySeconds * 1000);

        await Task.Delay(delayMs);
        await Awaitable.MainThreadAsync();

        retryAction();
    }

    public void Show()
    {
        if (LoadedAd != null && LoadedAd.CanShowAd())
        {
            AdService.EnterAdMode();
            LoadedAd.Show(async reward =>
            {
                await Awaitable.MainThreadAsync();
                Debug.Log("?");
                OnRewardedAction?.Invoke();
            });
        }
        else
        {
            OnFailedAction?.Invoke();
            LoadAd();
        }
    }

    public void OnClosed()
    {
        OnClosedAction?.Invoke();
        Cleanup();
    }

    public void OnFailed(AdError _)
    {
        OnFailedAction?.Invoke();
        Cleanup();
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
