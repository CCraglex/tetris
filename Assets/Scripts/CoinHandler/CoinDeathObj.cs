
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CoinDeathObj : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointText;
    [SerializeField] private float flashTimer;
    [SerializeField] private int flashCount;
    [SerializeField] private float moveSpeed;

    public void Start()
    {
        DOTween.Sequence()
            .Append(pointText.DOFade(0.25f, flashTimer))
            .Append(pointText.DOFade(1f, flashTimer))
            .SetLoops(flashCount)
            .OnStart(() =>
            {
                var rect = pointText.transform as RectTransform;
                rect.DOAnchorPosY(rect.anchoredPosition.y + 1000, flashCount * flashTimer * 2);
            })
            .OnComplete(() => {
                var rect = pointText.transform as RectTransform;
                rect.DOKill();
                Destroy(gameObject);
            });
    }
}