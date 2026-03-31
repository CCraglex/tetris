
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class WinPanel : MonoBehaviour
{
    [SerializeField] private LevelLoadManager levelLoader;
    [SerializeField] private LevelGameplay gameplayData;
    [SerializeField] private TextMeshProUGUI offerText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private ParticleSystem particles;

    [Header("Canvases")]
    [SerializeField] private CanvasGroup menuCanvas;
    [SerializeField] private CanvasGroup winCanvas;
    [SerializeField] private CanvasGroup swapCanvas;
    [SerializeField] private CanvasGroup gameCanvas;

    [Header("Animation Settings")]
    public float duration = 0.4f;
    public float jumpHeight = 30f;
    public int jumpCount = 1;

    public void PlayPopup(RectTransform target)
    {
        Vector2 originalPos = target.anchoredPosition;
        Sequence seq = DOTween.Sequence();

        seq.Join(
            target.DOScale(Vector3.one, duration)
                  .SetEase(Ease.OutBack)
        );

        seq.Join(
            target.DOAnchorPosY(originalPos.y + jumpHeight, duration * 0.5f)
                  .SetEase(Ease.OutQuad)
        );

        seq.Append(
            target.DOAnchorPosY(originalPos.y, duration * 0.5f)
                  .SetEase(Ease.InQuad)
        );
    }
    
    [SerializeField] private List<RectTransform> uiObjects;
    [SerializeField] private float timeBetweenObjects;


    public void OnEnable()
    {
        IEnumerator PlayAnim()
        {
            offerText.text = $"Earned <sprite index= 0>: {gameplayData.collectedCashThisRound}\n<size=50%>Watch an ad to double?</size>";
            rewardText.gameObject.SetActive(false);
            var waitTimer = new WaitForSeconds(timeBetweenObjects);

            foreach (var rect in uiObjects)
            {
                yield return waitTimer;
                PlayPopup(rect);
            }
        }

        StartCoroutine(PlayAnim());
    }

    public void OnDisable()
    {
        foreach (var item in uiObjects)
            item.localScale = Vector3.zero;
    }

    public void RewardPlayer()
    {
        bool didWatchAd = true;

        IEnumerator DelayedReward()
        {
            yield return new WaitForSeconds(0.5f);
            rewardText.gameObject.SetActive(true);
            PlayPopup(rewardText.rectTransform);
            particles.Play();
        }
        if (didWatchAd)
        {
            StartCoroutine(DelayedReward());
            SaveStateHandler.AddCash(gameplayData.collectedCashThisRound);
        }
    }


    private IEnumerator ILoadLevelAction(int level)
    {
        winCanvas.blocksRaycasts = false;

        yield return swapCanvas.DOFade(1,0.65f)
            .WaitForCompletion();

        var handle = levelLoader.CreateLevel(level);
        yield return new WaitUntil(() => handle.IsCompleted);

        winCanvas.alpha = 0;
        gameCanvas.alpha = 1;
        gameCanvas.blocksRaycasts = true;

        levelLoader.ReadyLevel(level);
        yield return swapCanvas.DOFade(0,0.65f)
            .WaitForCompletion();            
        
    }

    public void ReloadLevel()
        => StartCoroutine(ILoadLevelAction(gameplayData.lastLoadedLevel));

    public void NextLevel()
        => StartCoroutine(ILoadLevelAction(gameplayData.lastLoadedLevel + 1));

    public void MenuButton()
    {
        IEnumerator IAction()
        {
            winCanvas.blocksRaycasts = false;

            yield return swapCanvas.DOFade(1,0.65f)
                .WaitForCompletion();

            yield return new WaitForSeconds(0.25f);
            winCanvas.alpha = 0;
            menuCanvas.alpha = 1;
            menuCanvas.blocksRaycasts = true;

             yield return swapCanvas.DOFade(0,0.65f)
                .WaitForCompletion();            
        }

        StartCoroutine(IAction());
    }
}

