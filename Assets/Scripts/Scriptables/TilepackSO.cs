
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName ="Level Tile Pack")]
public class LevelTilepackSO : ScriptableObject
{
    public string displayName;
    public string Description;
    
    public TileBase Wall;
    public TileBase Flag;

    
}