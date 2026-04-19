using System;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdService : MonoBehaviour
{
    public bool IsTest = true;

    public static bool adsInitialized;

    private static bool isAdShowing;
    private static bool isAppPaused;

    [Header("Ad Unit IDs")]
    public string TestIDInt;
    public string TestIDRew;

    public string InterstitialID;
    public string RewardedReviveID;
    public string RewardedCoinID;

    public InterstitialAdHandler interstitial;
    private static float lastAdTimeInterstitial;

    public RewardedAdHandler rewardedCoin;
    public RewardedAdHandler rewardedRevive;

    public static AdService instance;

    

    public static void Init(AdService A)
    {
        instance = A;
        DontDestroyOnLoad(A.gameObject);
        
        var requestConfiguration = new RequestConfiguration
        {
            TestDeviceIds = new List<string>
            {
                "BE0C2407EEDB38D71F5FB6B1E1CBBFD4"
            }
        };

    MobileAds.SetRequestConfiguration(requestConfiguration);

        MobileAds.Initialize(status =>
        {
            if (status == null)
            {
                Debug.LogError("Ads init failed");
                return;
            }

            adsInitialized = true;
            Debug.Log("Ads initialized");
            instance.SetupAds();
        });
    }

    private void SetupAds()
    {
        // Interstitial
        interstitial = new InterstitialAdHandler
        {
            ID = IsTest ? TestIDInt : InterstitialID
        };

        interstitial.OnClosedAction += ExitAdMode;
        interstitial.OnFailedAction += ExitAdMode;

        // Rewarded - Coin
        rewardedCoin = new RewardedAdHandler
        {
            ID = IsTest ? TestIDRew : RewardedCoinID
        };

        rewardedCoin.OnClosedAction += ExitAdMode;
        rewardedCoin.OnFailedAction += ExitAdMode;

        // Rewarded - Revive
        rewardedRevive = new RewardedAdHandler
        {
            ID = IsTest ? TestIDRew : RewardedReviveID
        };

        rewardedRevive.OnClosedAction += ExitAdMode;
        rewardedRevive.OnFailedAction += ExitAdMode;

        // Preload all
        interstitial.LoadAd();
        rewardedCoin.LoadAd();
        rewardedRevive.LoadAd();
    }

    // -----------------------
    // Public API
    // -----------------------

    public static void ShowInterstitial()
    {
        if (instance == null) return;
        
        bool randChance = UnityEngine.Random.Range(0, 100) < 75;
        bool isAdReady = adsInitialized && instance.interstitial.LoadedAd != null && instance.interstitial.LoadedAd.CanShowAd();
        bool timeReady = Time.time - lastAdTimeInterstitial > 60f;
        if (randChance && isAdReady && timeReady)
        {
            lastAdTimeInterstitial = Time.time;
            EnterAdMode();
            instance.interstitial.Show();
        }
    }

    public static void ShowRewardedCoin()
    {
        if (instance == null) return;

        EnterAdMode();
        instance.rewardedCoin.Show();
    }

    public static void ShowRewardedRevive()
    {
        if (instance == null) return;

        EnterAdMode();
        instance.rewardedRevive.Show();
    }

    // -----------------------
    // Ad Mode Control
    // -----------------------

    public static void EnterAdMode()
    {
        if (isAdShowing) return;

        isAdShowing = true;
        SoundService.PauseAll();
        Time.timeScale = 0f;
    }

    public static async void ExitAdMode()
    {
        await Awaitable.MainThreadAsync();

        if (!isAdShowing) return;

        isAdShowing = false;
        SoundService.ResumeAll();
        Time.timeScale = 1f;
    }

    // -----------------------
    // App Pause Handling
    // -----------------------

    private void OnApplicationPause(bool pause)
    {
        isAppPaused = pause;

        if (pause)
        {
            if (isAdShowing) return;

            SoundService.PauseAll();
            Time.timeScale = 0f;
        }
        else
        {
            if (isAdShowing) return;

            SoundService.ResumeAll();
            Time.timeScale = 1f;
        }
    }
}