using UnityEngine;

public class GamePlayButtons : MonoBehaviour
{
    public PlayerMovement P;
    public LevelGameplay levelGameplay;

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
        if(!SaveStateHandler.HasPowerup())
            return;

        SaveStateHandler.UsePowerup();
        StartCoroutine(levelGameplay.ActivatePowerup());
    }
}