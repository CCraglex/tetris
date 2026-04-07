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

    [SerializeField] private CanvasGroup winCanvas;
    [SerializeField] private CanvasGroup swapCanvas;
    [SerializeField] private CanvasGroup gameCanvas;

    [SerializeField] private CanvasGroup powerupElement;
    [SerializeField] private Image powerupImage;

    [SerializeField] private float powerupTimer;

    [SerializeField] LevelCollisionHandler collisionHandler;

    [SerializeField] private LevelTextHandler levelText;
    [SerializeField] private Player player;
    [SerializeField] private LevelCamera levelCamera;

    [SerializeField] private LevelHypeTextHandler hypeText;
    [SerializeField] private TextMeshProUGUI skillCountText;

    private void Start()
    {
        skillCountText.text = SaveStateHandler.GetPowerupCount().ToString();
    }

    private IEnumerator TickTimer(int levelID)
    {
        levelText.gameObject.SetActive(true);
        levelText.SetLevel(levelID);
        yield return levelText.PlayBeginning();
    }

    public IEnumerator StartPlayingLevel(int levelID)
    {
        lastLoadedLevel = levelID;
        yield return TickTimer(levelID);
        levelText.StartCoroutine(levelText.PlayAnim());

        player.StartPlaying();
        levelCamera.StartAction();
    }

    public IEnumerator ActivatePowerup()
    {
        skillCountText.text = SaveStateHandler.GetPowerupCount().ToString();
        remainingPowerupTime = powerupTimer;
        float warningTime = 2f;
        powerupElement.alpha = 1;

        float fastSpeed = 0.15f;
        float slowSpeed = 0.6f;
        
        Tween fadeTween;

        void StartFadeLoop()
        {
            float targetAlpha = powerupImage.color.a > 0.8f ? 0.6f : 1f;
            float speed = remainingPowerupTime < warningTime ? fastSpeed : slowSpeed;

            fadeTween = powerupImage.DOFade(targetAlpha, speed)
                .SetEase(Ease.Linear)
                .OnComplete(StartFadeLoop);
        }

        StartFadeLoop();
        while(remainingPowerupTime > 0)
        {
            yield return null;
            remainingPowerupTime -= Time.deltaTime;
            powerupImage.fillAmount = remainingPowerupTime / powerupTimer;
        }

        powerupElement.alpha = 0;
        powerupImage.DOKill();
    }

    public bool OnPlayerDeath()
    {
        if(remainingPowerupTime > 0)
            return false;
        
        levelCamera.DoFollow = false;
        StartCoroutine(OnGameOver());
        return true;
    }

    public void OnPlayerWon()
    {
        IEnumerator WinSequence()
        {
            yield return StartCoroutine(hypeText.PlayWin());
            gameCanvas.blocksRaycasts = false;

            yield return swapCanvas.DOFade(1,0.65f)
                .WaitForCompletion();

            yield return new WaitForSeconds(0.25f);
            gameCanvas.alpha = 0;
            levelLoader.ClearMap();
            player.ClearTiles();
            
            winCanvas.alpha = 1;
            winCanvas.blocksRaycasts = true;

             yield return swapCanvas.DOFade(0,0.65f)
                .WaitForCompletion();  
        }
        StartCoroutine(WinSequence());
    }

    public IEnumerator OnGameOver()
    {
        bool revived = false;
        yield return StartCoroutine(loseCanvas.WaitResponse(() => revived = true));

        if (revived)
        {
            yield return StartCoroutine(StartPlayingLevel(lastLoadedLevel));
            yield return ActivatePowerup();
        }
        else {
            yield return levelLoader.CreateLevel(lastLoadedLevel);
            levelLoader.ReadyLevel(lastLoadedLevel);
        }
    }
}