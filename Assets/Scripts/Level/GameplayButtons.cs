using System;
using UnityEngine;

public class GamePlayButtons : MonoBehaviour
{
    public Player player;
    public PlayerMovement P;
    public LevelGameplay levelGameplay;

    [SerializeField] private PausePanel pausePanel;
    [SerializeField] private ShopPanel shopPanel;

    public void TurnRight()
        => P.RotateRight();
    
    public void TurnLeft()
        => P.RotateLeft();
    
    public void MoveRight()
        => P.MoveRight();
    
    public void MoveLeft()
        => P.MoveLeft();
    
    public void SkillButton()
    {
        if(levelGameplay.PausingBlocked())
            return;
            
        if(SaveStateHandler.GetPowerupCount() > 0)
        {
            SaveStateHandler.UsePowerup();
            StartCoroutine(levelGameplay.ActivatePowerup());
            return;
        }

        if (SaveStateHandler.GetCash() > 30)
        {
            SaveStateHandler.RemoveCash(30);
            SaveStateHandler.AddPowerup(3);
            SkillButton();
            return;
        }

        pausePanel.Pause();
        shopPanel.ShowPanel();
    }
}