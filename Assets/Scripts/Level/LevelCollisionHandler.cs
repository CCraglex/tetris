
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CollisionTileType
{
    None,
    Flag,
    Coin,
    Wall
}
public class LevelCollisionHandler : MonoBehaviour
{
    public List<Vector2Int> playerTiles;
    public List<Vector2Int> wallGridPositions;
    public List<Vector2Int> flagGridPositions;
    public List<Vector2Int> coinGridPositions;

    public void SetupData(Player player,LevelSO levelData)
    {
        wallGridPositions = levelData.Tiles
            .Where(tile => tile.tileToUse == "Wall")
            .Select(tile => new Vector2Int(tile.position.x,tile.position.y))
            .ToList();
        
        flagGridPositions = levelData.Tiles
            .Where(tile => tile.tileToUse == "Flag")
            .Select(tile => new Vector2Int(tile.position.x,tile.position.y))
            .ToList();
        
        playerTiles = player.GeneratePlayer();
    }

    public void UpdateCoins(List<Vector2Int> positions)
        => coinGridPositions = positions;

    public bool CanLandHere(Vector2 pPos)
    {
        Vector2Int playerPos = new(
            Mathf.RoundToInt(pPos.x),
            Mathf.RoundToInt(pPos.y)
        );

        var hittingTiles = new List<Vector2Int>();

        foreach (var playerTile in playerTiles)
        {
            Vector2Int lowerTile = playerPos + new Vector2Int(playerTile.x, playerTile.y - 1);

            if (wallGridPositions.Contains(lowerTile))
                hittingTiles.Add(lowerTile);
        }

        return hittingTiles.Count > 0;
    }

    private List<Vector2Int> GetCollisionList(CollisionTileType type)
    {
        switch (type)
        {
            case CollisionTileType.Flag:
                return flagGridPositions;
            case CollisionTileType.Coin:
                return coinGridPositions;
            case CollisionTileType.Wall:
                return wallGridPositions;

            default:
                Debug.Log("No collision found.");
                return null;
        }
    }
    
    public bool IsCollidingWith(CollisionTileType tile, Vector2 pPos, out List<Vector2Int> collisionCells)
    {
        Vector2Int playerPos = new(
            Mathf.RoundToInt(pPos.x),
            Mathf.RoundToInt(pPos.y)
        );

        List<Vector2Int> tiles = GetCollisionList(tile);

        var cols = playerTiles
            .Select(pos => playerPos + pos)
            .Where(worldPos => tiles.Contains(worldPos))
            .ToList();

        bool any = cols.Count > 0;

        collisionCells = any ? cols : null;

        return any;
    }
}