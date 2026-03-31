
using System;
using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class LevelGameplay : MonoBehaviour
{
    public bool isPowerUpOn;
    public int collectedCashThisRound;
    public int lastLoadedLevel;

    [SerializeField] private LevelLoadManager levelLoader;

    [SerializeField] private CanvasGroup winCanvas;
    [SerializeField] private CanvasGroup swapCanvas;
    [SerializeField] private CanvasGroup gameCanvas;

    [SerializeField] private float powerupTimer;

    [SerializeField] LevelCollisionHandler collisionHandler;

    [SerializeField] private LevelTextHandler levelText;
    [SerializeField] private Player player;
    [SerializeField] private LevelCamera levelCamera;

    [SerializeField] private LevelHypeTextHandler hypeText;

    private IEnumerator TickTimer(int levelID)
    {
        levelText.gameObject.SetActive(true);
        levelText.SetLevel(levelID);
        yield return levelText.PlayBeginning();
    }

    public IEnumerator StartPlayingLevel(int levelID)
    {
        lastLoadedLevel = levelID;
        collectedCashThisRound = 0;

        yield return TickTimer(levelID);
        levelText.StartCoroutine(levelText.PlayAnim());

        player.StartPlaying();
        levelCamera.StartAction();
    }

    public void OnGameOver()
    {
        print("Lose!");
    }


    public void OnPlayerDeath()
    {
        if(!isPowerUpOn)
            OnGameOver();
        else
            print("Powerup!");
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

    public IEnumerator ActivatePowerup()
    {
        isPowerUpOn = true;
        yield return new WaitForSeconds(powerupTimer);
        isPowerUpOn = false;
    }
}