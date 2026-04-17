using System;
using System.Threading.Tasks;
using GoogleMobileAds.Api;
using UnityEngine;

public interface IAd
{
    public void LoadAd(int attempt = 0);
    public void Show();
    public void OnClosed();
    public void OnFailed(AdError _);
    public void Cleanup();
}
