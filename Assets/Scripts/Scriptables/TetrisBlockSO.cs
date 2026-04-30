
using UnityEngine;

[CreateAssetMenu(menuName ="Tetris block")]
public class TetrisBlockSO : ScriptableObject
{
    public Vector2 pivot;
    public Vector2Int[] localPositions; 
}