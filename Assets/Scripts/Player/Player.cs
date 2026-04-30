using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Sprite tileVisual;

    [SerializeField] private LevelCollisionHandler collisionHandler;
    [SerializeField] private LevelGameplay levelGameplay;

    private bool playerActed;
    public Vector2 pivot;

    private void CreateTile(Vector2 spot)
    {
        var tile = Instantiate(tilePrefab, Vector2.zero, Quaternion.identity, transform);
        tile.transform.position = spot;
        tile.GetComponent<SpriteRenderer>().sprite = tileVisual;
    }

    public void ClearTiles()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }

    public List<Vector2Int> GeneratePlayer()
    {
        ClearTiles();
        var Data = AssetLoader.GetCurrentLevel().GetPlayerTiles(out Vector2 p);
        pivot = p;
        
        transform.position = Data[0] + pivot;

        Vector2Int[] gridData = new Vector2Int[Data.Length];

        for (int i = 0; i < Data.Length; i++)
        {
            gridData[i] = new Vector2Int((int)Data[i].x,(int)Data[i].y);
            CreateTile(gridData[i]);
        }
            
        movement.InitTiles(gridData);
        return gridData.ToList();
    }

    public void StartPlaying()
        => StartCoroutine(IGameLoop());

    public void StopPlaying()
        => movement.Active = false;

    public IEnumerator IGameLoop()
    {
        movement.Active = true;
        
        float timePerStep = 1 / AssetLoader.GetCurrentLevel().TimePerStep;
        var wait = new WaitForSeconds(timePerStep);
        
        float lockTimer = 0;
        
        while (movement.Active)
        {
            if (collisionHandler.CanLandHere())
            {
                lockTimer += Time.deltaTime;

                if (playerActed)
                {
                    lockTimer = 0f;
                    playerActed = false;
                }

                if (lockTimer >= timePerStep)
                {
                    if(collisionHandler.IsCollidingWith(CollisionTileType.Flag,out _))
                    {
                        levelGameplay.OnPlayerWon();
                        movement.Active = false;
                    }
                    else
                    {
                        bool actualDeath = levelGameplay.OnPlayerDeath();
                        if(actualDeath)
                            movement.Active = false;
                    }
                }
                yield return null;
            }
            else
            {
                movement.MoveDown();
                lockTimer = 0f;
                yield return wait;
            }
        }
    }
}
