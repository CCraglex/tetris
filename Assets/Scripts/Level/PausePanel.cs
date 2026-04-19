
using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    [SerializeField] private Menu menu;
    [SerializeField] private LevelGameplay gameplay;
    [SerializeField] private Player player;
    [SerializeField] private LevelCamera camera2D;

    [SerializeField] private Image sfxImg;
    [SerializeField] private Image musImg;

    [SerializeField] private Sprite musOff;
    [SerializeField] private Sprite musOn;
    [SerializeField] private Sprite sfxOn;
    [SerializeField] private Sprite sfxOff;

    [SerializeField] private CanvasGroup swapCanvas;
    [SerializeField] private CanvasGroup menuCanvas;
    [SerializeField] private CanvasGroup pauseCanvas;
    [SerializeField] private CanvasGroup gameCanvas;

    [SerializeField] private LevelLoadManager levelLoader;

    [SerializeField] private RectTransform pauseRect;
    [SerializeField] private GameObject blurRect;

    public bool isOpen;

    public void SetSFX()
    {
        bool v = PlayerPrefs.GetInt("Sfx") == 1;
        sfxImg.sprite = v ? sfxOn : sfxOff;
        PlayerPrefs.SetInt("Sfx",v ? 1 : 0);
        SaveStateHandler.Sfx = v;
    }

    public void SetSong()
    {
        bool v = PlayerPrefs.GetInt("Mus") == 1;
        musImg.sprite = v ? musOn : musOff;
        PlayerPrefs.SetInt("Mus",v ? 1 : 0);
        SaveStateHandler.Mus = v;
    }

    public void Pause()
    {
        isOpen = true;
        player.StopPlaying();
        camera2D.EndAction();
    }

    public void Continue()
    {   
        isOpen = false;
        pauseCanvas.alpha = 0;
        pauseCanvas.blocksRaycasts = false;
        StartCoroutine(gameplay.ICountDown());
    }

    public void PauseButton()
    {
        if(isOpen)
            return;

        Pause();

        pauseCanvas.blocksRaycasts = true;
        pauseCanvas.interactable = true;

        blurRect.SetActive(true);

        pauseCanvas.alpha = 1f;

        Vector2 startPos = new Vector2(0, Screen.height);
        pauseRect.anchoredPosition = startPos;

        Sequence seq = DOTween.Sequence();

        seq.Append(
            pauseRect.DOAnchorPos(Vector2.zero, 0.5f)
            .SetEase(Ease.OutCubic)
        );

        seq.Join(
            pauseRect.DOPunchScale(Vector3.one * 0.1f, 0.4f, 8, 0.8f)
        );

        seq.Join(
            pauseCanvas.DOFade(1f, 0.2f)
        );
    }

    public void ReturnToMenu()
    {      
        IEnumerator IAction()
        {
            pauseCanvas.blocksRaycasts = false;

            yield return swapCanvas.DOFade(1,0.65f)
                .WaitForCompletion();
                
            gameplay.ClearGame();
            gameplay.levelText.HideText();
            gameCanvas.alpha = 0;
            gameCanvas.blocksRaycasts = false;

            pauseCanvas.alpha = 0;
            pauseCanvas.blocksRaycasts = false;

            menuCanvas.alpha = 1;
            menuCanvas.blocksRaycasts = true;

            yield return new WaitForSeconds(0.25f);
            yield return swapCanvas.DOFade(0,0.65f)
                .WaitForCompletion();   

            isOpen = false;         
        }

        StartCoroutine(IAction());
    }

    public void RestartLevel()
    {
        IEnumerator IAction()
        {
            pauseCanvas.blocksRaycasts = false;

            yield return swapCanvas.DOFade(1,0.65f)
                .WaitForCompletion();

            var handle = levelLoader.CreateLevel(gameplay.lastLoadedLevel);
            yield return new WaitUntil(() => handle.IsCompleted);

            pauseCanvas.alpha = 0;
            gameCanvas.alpha = 1;
            gameCanvas.blocksRaycasts = true;

            yield return swapCanvas.DOFade(0,0.65f)
                .WaitForCompletion();

            isOpen = false;
            gameplay.remainingPowerupTime = 0;
            yield return StartCoroutine(gameplay.ICountDown());
        }

        StartCoroutine(IAction());
    }
}