using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelGameplay : MonoBehaviour
{
    public float remainingPowerupTime;
    public int collectedCashThisRound;
    public int lastLoadedLevel;

    [SerializeField] private LevelLoadManager levelLoader;
    [SerializeField] private LosePanel loseCanvas;
    [SerializeField] private PausePanel pausePanel;
    [SerializeField] private WinPanel winPanel;

    [SerializeField] private CanvasGroup winCanvas;
    [SerializeField] private CanvasGroup swapCanvas;
    [SerializeField] private CanvasGroup gameCanvas;

    [SerializeField] private CanvasGroup powerupElement;
    [SerializeField] private Image powerupImage;

    [SerializeField] private float powerupTimer;

    [SerializeField] LevelCollisionHandler collisionHandler;

    public LevelTextHandler levelText;
    public Player player;
    public LevelCamera levelCamera;

    [SerializeField] private LevelHypeTextHandler hypeText;
    [SerializeField] private TextMeshProUGUI skillCountText;

    private bool countDown1;
    private bool countDown2;

    private Tween powerupTween;

    private void Start()
    {
        skillCountText.text = SaveStateHandler.GetPowerupCount().ToString();
        SaveStateHandler.PowerupChanged += (val) => skillCountText.text = $"{val}";
    }

    public IEnumerator ICountDown()
    {
        pausePanel.isOpen = false;
        countDown1 = true;
        countDown2 = true;

        yield return levelText.StartCoroutine(levelText.ICountDown(() => countDown1 = false));
        if (countDown1)
            yield break;

        player.StartPlaying();
        levelCamera.StartAction();
        
        StartCoroutine(levelText.IMoveUpward(() => countDown2 = false));
        if (countDown2)
            yield break;
        
        
    }

    public void StartPlayingLevel(int levelID)
    {
        player.StopPlaying();
        levelCamera.EndAction();
        lastLoadedLevel = levelID;
        StartCoroutine(ICountDown());
        remainingPowerupTime = 0;
    }

    public IEnumerator ActivatePowerup()
    {
        remainingPowerupTime = powerupTimer;
        float warningTime = 2f;
        powerupElement.alpha = 1;

        float fastSpeed = 0.15f;
        float slowSpeed = 0.6f;

        if(powerupTween != null && powerupTween.IsPlaying())
            yield break;
            
        void StartFadeLoop()
        {
            float targetAlpha = powerupImage.color.a > 0.8f ? 0.6f : 1f;
            float speed = remainingPowerupTime < warningTime ? fastSpeed : slowSpeed;

            powerupTween = powerupImage.DOFade(targetAlpha, speed)
                .SetEase(Ease.Linear)
                .OnComplete(StartFadeLoop);
        }

        StartFadeLoop();

        while(remainingPowerupTime > 0 && powerupTween.IsPlaying())
        {
            yield return null;
            if(pausePanel.isOpen || countDown1)
                continue;

            remainingPowerupTime -= Time.deltaTime;
            powerupImage.fillAmount = remainingPowerupTime / powerupTimer;
        }

        powerupElement.alpha = 0;
        powerupImage.DOKill();
        powerupTween = null;
    }

    public bool OnPlayerDeath()
    {
        if(remainingPowerupTime > 0)
            return false;

        pausePanel.isOpen = true;
        levelCamera.DoFollow = false;
        OnGameOver();
        return true;
    }

    public void OnPlayerWon()
    {
        IEnumerator WinSequence()
        {
            pausePanel.isOpen = true;
            SaveStateHandler.FinishLevel(lastLoadedLevel);
            yield return StartCoroutine(hypeText.PlayWin());
            gameCanvas.blocksRaycasts = false;

            yield return swapCanvas.DOFade(1,0.65f)
                .WaitForCompletion();

            yield return new WaitForSeconds(0.25f);
            ClearGame();
            winPanel.Enable();
            gameCanvas.alpha = 0;            
            winCanvas.alpha = 1;
            winCanvas.blocksRaycasts = true;

             yield return swapCanvas.DOFade(0,0.65f)
                .WaitForCompletion();  
        }
        StartCoroutine(WinSequence());
    }

    public void ClearGame()
    {
        levelLoader.ClearMap();
        levelText.lastID = 0;
        player.ClearTiles();
        player.StopPlaying();
        levelCamera.EndAction();
    }

    private IEnumerator IRevive()
    {
        yield return StartCoroutine(ICountDown());
        StartCoroutine(ActivatePowerup());
    }

    private IEnumerator IRestart()
    {
        yield return levelLoader.CreateLevel(lastLoadedLevel);
        levelLoader.ReadyLevel(lastLoadedLevel);
    }

    public void OnGameOver()
    {
        loseCanvas.ShowReviveUI(revived =>
        {
            if (revived)
                StartCoroutine(IRevive());
            else
                StartCoroutine(IRestart());
        });
    }
}