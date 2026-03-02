using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Sprite tileVisual;

    private bool playerActed;

    private void CreateTile(Vector2 spot)
    {
        var tile = Instantiate(tilePrefab, Vector2.zero, Quaternion.identity, transform);
        tile.transform.localPosition = spot;
        tile.GetComponent<SpriteRenderer>().sprite = tileVisual;
    }

    private void ClearTiles()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }

    public void GeneratePlayer()
    {
        ClearTiles();
        var Data = AssetLoader.GetCurrentLevel().GetPlayerTiles(out Vector2 spawn);
        transform.position = spawn;
        movement.InitTiles(Data);
        LevelHandler.playerTiles = Data.ToList();
        CreateTile(Vector2.zero);
        for (int i = 0; i < Data.Length - 1; i++)
            CreateTile(Data[i]);
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
            if (LevelHandler.CanLandHere())
            {
                lockTimer += Time.deltaTime;

                if (playerActed)
                {
                    lockTimer = 0f;
                    playerActed = false;
                }

                if (lockTimer >= timePerStep)
                {
                    if(LevelHandler.HasLandedOnFlag())
                        print("Win!");
                    else
                        print("Lose!");

                    movement.Active = false;
                    yield break;
                }

                yield return null; // check again next frame
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
