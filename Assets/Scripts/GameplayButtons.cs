
using UnityEngine;

public class GamePlayButtons : MonoBehaviour
{
    public PlayerMovement P;

    public void TurnRight()
        => P.RotateRight();
    
    public void TurnLeft()
        => P.RotateLeft();
    
    public void MoveRight()
        => P.MoveRight();
    
    public void MoveLeft()
        => P.MoveLeft();
}