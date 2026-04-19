using System;
using UnityEngine;

public class ShopPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup Panel;
    [SerializeField] private PausePanel pausePanel;

    public void ShowPanel()
    {
        Panel.alpha = 1;
        Panel.blocksRaycasts = true;
        pausePanel.Pause();
    }

    public void HidePanel()
    {
        Panel.alpha = 0;
        Panel.blocksRaycasts = false;
        pausePanel.Continue();
    }

    private void CleanupAd()
    {
        AdService.instance.rewardedCoin.OnRewardedAction -= AdSuccess;
        AdService.instance.rewardedCoin.OnClosedAction -= AdFail;
        AdService.instance.rewardedCoin.OnFailedAction -= AdFail;
        AdService.instance.rewardedCoin.Cleanup();
    }

    private void AdSuccess()
    {
        SaveStateHandler.AddPowerup(1);
        CleanupAd();
    }

    private void AdFail()
    {
        CleanupAd();
    }

    public void AdButton()
    {
        AdService.instance.rewardedCoin.OnRewardedAction += AdSuccess;
        AdService.instance.rewardedCoin.OnClosedAction += AdFail;
        AdService.instance.rewardedCoin.OnFailedAction += AdFail;
        AdService.ShowRewardedCoin();
    }

    public void BuyButton(int amount)
    {
        SaveStateHandler.AddCash(amount);
    }
}
