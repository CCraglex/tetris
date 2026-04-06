
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

    public Button button;

    public float moveDuration = 0.7f;
    public float fillDuration = 3f;

    public Action ReviveRewardClaimed;
    public Action ReviveRewardGained;

    public IEnumerator WaitResponse(Action A)
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

        bool adTrigger = false;
        bool adCompleted = false;
        bool buttonAnimCompleted = false;

        seq.Append(buttonImg.DOFillAmount(0f, fillDuration)
            .SetEase(Ease.InOutSine))
            .OnComplete(() => buttonAnimCompleted = true);

        buttonImg.transform.DOScale(1.05f, 0.6f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
        
        void a1() => adTrigger = true;
        void a2() => adCompleted = true;

        ReviveRewardClaimed += a1;
        ReviveRewardGained += a2;

        try
        {
            yield return new WaitUntil(() => adTrigger || buttonAnimCompleted);

            if (buttonAnimCompleted)
                yield break;

            yield return new WaitUntil(() => adCompleted);
            A.Invoke();
        }
        finally
        {
            ReviveRewardClaimed -= a1;
            ReviveRewardGained -= a2;
            StartCoroutine(IClearPanel());
        }
    }

    public void AdButton()
    {
        print("?");
        ReviveRewardClaimed?.Invoke();
        ReviveRewardGained?.Invoke();
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