
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool Active;
    public bool EnableDebug;

    [SerializeField] private LevelCollisionHandler collisionHandler;
    [SerializeField] private CoinHandler coinHandler;

    private Transform Player;
    private Vector2Int[] playerTiles;

    public void InitTiles(Vector2Int[] values)
        => playerTiles = values;

    private void Awake()
        => Player = transform;


    private bool TryMovingPlayer(Vector2Int[] tiles,Vector2Int direction)
    {
        if(!Active)
            return false;

        Vector2Int playerPos = new(
            Mathf.RoundToInt(Player.position.x),
            Mathf.RoundToInt(Player.position.y)
        );
       
        foreach (var tile in tiles)
        {
            Vector2Int pos = playerPos + tile + direction;        
            if (collisionHandler.wallGridPositions.Contains(pos) || IsTileOutOfBounds(pos.x))
                return false;
        }
        return true;
    }

    private void HandleCoinReward()
    {
        if(collisionHandler.IsCollidingWith(CollisionTileType.Coin,transform.position,out var positions))
        {           
            foreach (var p in positions)
                coinHandler.RewardCoin(p);
        }
    }

    public bool IsTileOutOfBounds(int x)
    {
        int width = AssetLoader.GetCurrentLevel().GetWidth();

        int minX = -(width / 2);
        int maxX = minX + width - 1;

        bool isOut = x < minX || x > maxX;
        return isOut;
    }


    public bool CanMoveDown()
        => TryMovingPlayer(playerTiles,Vector2Int.down);
    public bool CanMoveLeft()
        => TryMovingPlayer(playerTiles,Vector2Int.left);
    public bool CanMoveRight()
        => TryMovingPlayer(playerTiles,Vector2Int.right);


    public void MoveDown()
    {
        if(CanMoveDown())
            Player.position += Vector3.down;
        
        HandleCoinReward();
    }
        
    public void MoveLeft()
    {
        if(CanMoveLeft())
            Player.position += Vector3.left;
        
        HandleCoinReward();
    }

    public void MoveRight()
    {
        if(CanMoveRight())
            Player.position += Vector3.right;
        
        HandleCoinReward();
    }

    public bool CanRotateLeft(Vector2Int[] tiles,out Vector2Int kick)
    {
        List<Vector2Int> modified = tiles
            .Select(v => new Vector2Int(-v.y,v.x))
            .ToList();
        return TryRotatingPlayer(modified.ToArray(),out kick);
    }
        
    public bool CanRotateRight(Vector2Int[] tiles,out Vector2Int kick)
    {
        List<Vector2Int> modified = tiles
            .Select(v => new Vector2Int(v.y,-v.x))
            .ToList();
        return TryRotatingPlayer(modified.ToArray(),out kick);
    }
    private bool TryRotatingPlayer(Vector2Int[] rotatedTiles,out Vector2Int kickDir)
    {
        kickDir = Vector2Int.zero;
        if(!Active)
            return false;

        foreach (var kick in DefaultKicks)
        {
            kickDir = kick;
            if (TryMovingPlayer(rotatedTiles, Vector2Int.zero))
                return true;
        }
        return false;
    }

    public void RotateLeft()
    {
        var rotatedTiles = new Vector2Int[playerTiles.Length];
        for (int i = 0; i < rotatedTiles.Length; i++)
            rotatedTiles[i] = new Vector2Int(-playerTiles[i].y, playerTiles[i].x);

        if (!CanRotateLeft(rotatedTiles,out Vector2Int kickDir))
            return;
            
        transform.position += new Vector3(kickDir.x,kickDir.y,0);
        transform.Rotate(0, 0, 90);       
        playerTiles = rotatedTiles;
        collisionHandler.playerTiles = playerTiles.ToList();
    }

    public void RotateRight()
    {
        var rotatedTiles = new Vector2Int[playerTiles.Length];
        for (int i = 0; i < rotatedTiles.Length; i++)
            rotatedTiles[i] = new Vector2Int(playerTiles[i].y, -playerTiles[i].x);

        if(!CanRotateRight(rotatedTiles,out Vector2Int kickDir))
            return;
        
        transform.Rotate(0, 0, -90);
        transform.position += new Vector3(kickDir.x,kickDir.y,0);
        playerTiles = rotatedTiles;
        collisionHandler.playerTiles = playerTiles.ToList();
    }

    static readonly Vector2Int[] DefaultKicks =
    {
        new(0, 0),   // no kick
        new(1, 0),   // right
        new(-1, 0),  // left
        new(0, 1),   // up (floor kick)
    };
    
    public void OnDrawGizmos()
    {
        if(EnableDebug == false)
            return;

        Gizmos.color = Color.black;
        foreach (var item in playerTiles)
            Gizmos.DrawWireSphere(transform.position + new Vector3(item.x,item.y),0.25f);

        Gizmos.color = Color.red;
        foreach (var item in collisionHandler.wallGridPositions)
            Gizmos.DrawWireSphere(new Vector3(item.x,item.y),0.25f);
        
        Gizmos.color = Color.yellow;
        foreach (var item in collisionHandler.coinGridPositions)
            Gizmos.DrawWireSphere(new Vector3(item.x,item.y),0.25f);

        Gizmos.color = Color.green;
        foreach (var item in collisionHandler.flagGridPositions)
            Gizmos.DrawWireSphere(new Vector3(item.x,item.y),0.25f);
    }
}