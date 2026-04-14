using System;
using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class LevelTextHandler : MonoBehaviour
{
    [SerializeField] private PausePanel pausePanel;
    [SerializeField] private LevelGameplay levelGameplay;

    private TextMeshProUGUI text;
    private Vector2 StartPos;
    const float moveDistance = 1920 / 2 + 300 ;
    [SerializeField] private float moveTimer;
    public float tickTimer;

    public int lastID;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        var rect = transform as RectTransform;
        StartPos = rect.anchoredPosition;
    }

    public void SetLevel(int newLevel)
    {
        var rect = transform as RectTransform;
        rect.anchoredPosition = StartPos;
        text.text = $"Level\n{newLevel}";
        text.fontSize /= 3;
    }
    
    public IEnumerator ICountDown(Action A)
    {
        text.color = new Color(0.95f, 0.95f, 0.95f, 0.2f); 
        var rect = transform as RectTransform;
        rect.anchoredPosition = StartPos;
        text.text = "3";
        float timer = 3f * tickTimer;
        while (timer > 0)
        {
            yield return null;
            if (pausePanel.isOpen)
                yield break;

            text.text = Mathf.Ceil(timer / tickTimer).ToString();
            timer -= Time.deltaTime;
        }

        text.text = "0";
        A.Invoke();
    } 

    public IEnumerator IMoveUpward(Action A)
    {
        print("Move up");

        var rect = transform as RectTransform;
        rect.anchoredPosition = StartPos;

        float t = 0;
        float target = StartPos.y + moveDistance;

        while (rect.anchoredPosition.y < target)
        {
            yield return null;
            if (pausePanel.isOpen)
                yield break;
            
            t += Time.deltaTime;
            rect.anchoredPosition = Vector2.Lerp(StartPos,new(StartPos.x,target),t / 2.25f);
        }
    }

    public void HideText()
        => text.color = new(0,0,0,0);
}
