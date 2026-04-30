
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

    [SerializeField] private AudioClip tickClip;

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
       
        foreach (var tile in tiles)
        {
            Vector2Int pos = tile + direction;        
            if (collisionHandler.wallGridPositions.Contains(pos) || IsTileOutOfBounds(pos.x))
                return false;
        }
        return true;
    }

    private void HandleCoinReward()
    {
        if(collisionHandler.IsCollidingWith(CollisionTileType.Coin,out var positions))
        {           
            foreach (var p in positions)
                coinHandler.RewardCoin(p);
        }
    }

    private void UpdateCollisionAfterMove()
    {
        for (int i = 0; i < playerTiles.Length; i++)
        {
            Vector2 pos = transform.GetChild(i).position;
            playerTiles[i] = new Vector2Int((int)pos.x,(int)pos.y);
        }
        collisionHandler.playerTiles = playerTiles.ToList();
    }

    private void UpdateRotationPos()
    {
        for (int i = 0; i < playerTiles.Length; i++)
            transform.GetChild(i).position = new Vector2(playerTiles[i].x,playerTiles[i].y);
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
        if (CanMoveDown())
        {
            Player.position += Vector3.down;
            UpdateCollisionAfterMove();
            SoundService.PlaySound(tickClip,0.08f,-0.15f,0.15f);
        }
            
        HandleCoinReward();
    }
        
    public void MoveLeft()
    {
        if(CanMoveLeft())
        {
            Player.position += Vector3.left;
            UpdateCollisionAfterMove();
            SoundService.PlaySound(tickClip,0.08f,-0.15f,0.15f);
        }

        HandleCoinReward();
    }

    public void MoveRight()
    {
        if(CanMoveRight())
        {
            Player.position += Vector3.right;
            UpdateCollisionAfterMove();
            SoundService.PlaySound(tickClip,0.08f,-0.15f,0.15f);
        }
        HandleCoinReward();
    }

    public void RotateLeft()
    {
        var rotatedTiles = new Vector2Int[playerTiles.Length];

        for (int i = 0; i < rotatedTiles.Length; i++)
        {
            Vector3 localPos = transform.GetChild(i).localPosition;
            Vector3 localRotated = new Vector3(-localPos.y, localPos.x, 0);
            Vector3 worldPos = transform.TransformPoint(localRotated);
            rotatedTiles[i] = new Vector2Int(
                Mathf.RoundToInt(worldPos.x),
                Mathf.RoundToInt(worldPos.y)
            );
        }

        if (!TryMovingPlayer(rotatedTiles,Vector2Int.zero))
            return;
  
        playerTiles = rotatedTiles;
        collisionHandler.playerTiles = playerTiles.ToList();
        UpdateRotationPos();
    }

    public void RotateRight()
    {
        var rotatedTiles = new Vector2Int[playerTiles.Length];

        for (int i = 0; i < rotatedTiles.Length; i++)
        {
            Vector3 localPos = transform.GetChild(i).localPosition;
            Vector3 localRotated = new Vector3(localPos.y, -localPos.x, 0);
            Vector3 worldPos = transform.TransformPoint(localRotated);
            rotatedTiles[i] = new Vector2Int(
                Mathf.RoundToInt(worldPos.x),
                Mathf.RoundToInt(worldPos.y)
            );
        }

        if (!TryMovingPlayer(rotatedTiles,Vector2Int.zero))
            return;
  
        playerTiles = rotatedTiles;
        collisionHandler.playerTiles = playerTiles.ToList();
        UpdateRotationPos();
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
        if(EnableDebug == false || playerTiles == null)
            return;

        Gizmos.color = Color.blue;
        foreach (var item in playerTiles)
            Gizmos.DrawSphere(new Vector3(item.x,item.y),0.25f);

        Gizmos.color = Color.red;
        foreach (var item in collisionHandler.wallGridPositions)
            Gizmos.DrawSphere(new Vector3(item.x,item.y),0.25f);
        
        Gizmos.color = Color.yellow;
        foreach (var item in collisionHandler.coinGridPositions)
            Gizmos.DrawSphere(new Vector3(item.x,item.y),0.25f);

        Gizmos.color = Color.green;
        foreach (var item in collisionHandler.flagGridPositions)
            Gizmos.DrawSphere(new Vector3(item.x,item.y),0.25f);
    }
}