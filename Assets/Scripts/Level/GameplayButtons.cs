using System;
using UnityEngine;

public class GamePlayButtons : MonoBehaviour
{
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
        if(SaveStateHandler.HasPowerup())
        {
            SaveStateHandler.UsePowerup();
            StartCoroutine(levelGameplay.ActivatePowerup());
            return;
        }

        if (SaveStateHandler.HasCash(30))
        {
            SaveStateHandler.RemoveCash(30);
            SaveStateHandler.AddPowerup(1);
            SkillButton();
            return;
        }

        pausePanel.Pause();
        shopPanel.ShowPanel();
    }
}