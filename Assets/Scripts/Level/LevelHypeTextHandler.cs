
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class LevelHypeTextHandler : MonoBehaviour
{
    [SerializeField] private List<string> failTexts;
    [SerializeField] private List<string> winTexts;

    [SerializeField] private TextMeshProUGUI textRenderer;

    private string GetRandomString(List<string> texts)
        => texts[Random.Range(0,texts.Count)];

    public IEnumerator PlayText(string text)
    {
        textRenderer.text = text;

        RectTransform rect = textRenderer.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = textRenderer.GetComponent<CanvasGroup>();

        // Reset state
        canvasGroup.alpha = 0;
        rect.localScale = Vector3.zero;
        rect.anchoredPosition = Vector2.zero;

        Sequence seq = DOTween.Sequence();

        seq.Append(canvasGroup.DOFade(1, 0.3f)); // fade in
        seq.Join(rect.DOScale(1.2f, 0.4f).SetEase(Ease.OutBack)); // pop scale
        seq.Append(rect.DOScale(1f, 0.2f)); // settle

        seq.Join(rect.DOAnchorPosY(100f, 0.8f).SetEase(Ease.OutQuad)); // float up

        seq.AppendInterval(1.5f);

        seq.Append(canvasGroup.DOFade(0, 0.4f)); // fade out
        seq.Join(rect.DOScale(0.8f, 0.4f)); // slight shrink

        yield return seq.WaitForCompletion();
        canvasGroup.alpha = 0;
    } 

    public IEnumerator PlayLose()
        {yield return StartCoroutine(PlayText(GetRandomString(failTexts)));}

    public IEnumerator PlayWin() 
        {yield return StartCoroutine(PlayText(GetRandomString(winTexts)));}
}