
using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LosePanel : MonoBehaviour
{
    public RectTransform panel;
    public CanvasGroup canvasGroup; // for fade
    public Image buttonImg;

    public float moveDuration = 0.7f;
    public float fillDuration = 3f;

    private bool rewarded = false;
    private Action<bool> onSuccessCallback;

    private Tween PlayPanelAnimation()
    {
        float screenHeight = Screen.height;

        canvasGroup.blocksRaycasts = true;
        panel.anchoredPosition = new Vector2(0, -screenHeight);
        panel.localScale = new Vector3(0.85f, 0.85f, 1f);

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        buttonImg.fillAmount = 1f;

        Sequence seq = DOTween.Sequence();

        seq.Append(panel.DOAnchorPos(Vector2.zero, moveDuration)
            .SetEase(Ease.OutBack));

        seq.Join(panel.DOScale(1f, moveDuration)
            .SetEase(Ease.OutCubic));

        if (canvasGroup != null)
            seq.Join(canvasGroup.DOFade(1f, moveDuration * 0.8f));

        seq.Append(panel.DOPunchScale(new Vector3(0.08f, 0.08f, 0), 0.25f, 8, 0.6f));

        seq.Join(panel.DOAnchorPos(new Vector2(0, 20f), 0.15f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutSine));

        seq.Append(buttonImg.DOFillAmount(0f, fillDuration)
            .SetEase(Ease.InOutSine));

        buttonImg.transform.DOScale(1.05f, 0.6f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        return seq;
    }

    public void ShowReviveUI(Action<bool> onSuccess)
    {
        onSuccessCallback = onSuccess;
        var t = PlayPanelAnimation();
        t.OnComplete(() => OnAdClosed());
    }

    public void AdButton()
    {
        rewarded = false;
        var ad = AdService.instance.rewardedRevive;

        // IMPORTANT: subscribe BEFORE showing ad
        ad.OnRewardedAction += OnAdRewarded;
        ad.OnClosedAction += OnAdClosed;
        ad.OnFailedAction += OnAdClosed;

        AdService.ShowRewardedRevive();
    }

    private void OnAdRewarded()
    {
        rewarded = true;
    }

    private void OnAdClosed()
    {
        CleanupAdEvents();

        if (onSuccessCallback == null)
            return;

        onSuccessCallback.Invoke(rewarded);
        onSuccessCallback = null;
        StartCoroutine(IClearPanel());
    }


    private void CleanupAdEvents()
    {
        var ad = AdService.instance.rewardedRevive;

        ad.OnRewardedAction -= OnAdRewarded;
        ad.OnClosedAction -= OnAdClosed;
        ad.OnFailedAction -= OnAdClosed;
        ad.Cleanup();
    }

    private IEnumerator IClearPanel()
    {
        float screenHeight = Screen.height;

        Sequence seq = DOTween.Sequence();
        seq.Append(panel.DOAnchorPos(new Vector2(0, -screenHeight), moveDuration)
            .SetEase(Ease.InBack));

        seq.Join(panel.DOScale(0.85f, moveDuration)
            .SetEase(Ease.InCubic));

        if (canvasGroup != null)
            seq.Join(canvasGroup.DOFade(0f, moveDuration * 0.8f));

        yield return seq.WaitForCompletion();
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;
    }
}