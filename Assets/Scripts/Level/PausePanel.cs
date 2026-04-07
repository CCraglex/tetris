
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


    private void Awake()
    {
        SetSFX(PlayerPrefs.GetInt("Sfx") == 1);
        SetSong(PlayerPrefs.GetInt("Mus") == 1);
    }

    public void SetSFX(bool value)
    {
        bool v =  value == true;
        sfxImg.sprite = v ? sfxOn : sfxOff;
        PlayerPrefs.SetInt("Sfx",v ? 1 : 0);
        SaveStateHandler.Sfx = v;
    }

    public void SetSong(bool value)
    {
        bool v =  value == true;
        musImg.sprite = v ? musOn : musOff;
        PlayerPrefs.SetInt("Mus",v ? 1 : 0);
        SaveStateHandler.Mus = v;
    }

    public void PauseButton()
    {
        player.StopPlaying();
        camera2D.EndAction();

        //Tween
    }

    public void ReturnToMenu()
    {
        //TODO: Coins not handled, probably lingering
        IEnumerator IAction()
        {
            pauseCanvas.blocksRaycasts = false;

            yield return swapCanvas.DOFade(1,0.65f)
                .WaitForCompletion();

            yield return new WaitForSeconds(0.25f);
            pauseCanvas.alpha = 0;
            menuCanvas.alpha = 1;
            menuCanvas.blocksRaycasts = true;

             yield return swapCanvas.DOFade(0,0.65f)
                .WaitForCompletion();            
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

            levelLoader.ReadyLevel(gameplay.lastLoadedLevel);
            yield return swapCanvas.DOFade(0,0.65f)
                .WaitForCompletion();
        }

        StartCoroutine(IAction());
    }

    public void Continue()
    {
        StartCoroutine(gameplay.StartPlayingLevel(gameplay.lastLoadedLevel));
    }
}