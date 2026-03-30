
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class LevelGameplay : MonoBehaviour
{
    public bool isPowerUpOn;

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
            print("Load Win Scene");
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