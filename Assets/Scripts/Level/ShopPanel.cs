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

    public void AdButton()
    {
        SaveStateHandler.AddPowerup(1);
    }

    public void BuyButton(int amount)
    {
        SaveStateHandler.AddCash(amount);
    }
}
