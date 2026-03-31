using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;


[System.Serializable]
public class CoinInstance
{
    public GameObject Instance;
    public bool Collected;
    [SerializeField] Vector2 position;

    public Vector2 Position {
        get => Instance.transform.position; 
        set {
            position = value;
            Instance.transform.position = value;
        }
    }
}

public class CoinHandler : MonoBehaviour
{
    [SerializeField] private LevelGameplay coinCollectData;

    [SerializeField] private GameObject CoinPrefab;
    [SerializeField] int coinPoolCount;
    [SerializeField] Transform coinsParent;
    [SerializeField] private TextMeshProUGUI textRenderer;

    [SerializeField] private GameObject coinDeathAnim;
    [SerializeField] private Transform deathAnimParent;


    private Queue<CoinInstance> Coins;
    [SerializeField] List<CoinInstance> ActiveCoins;
    
    public void RewardCoin(Vector2 pos)
    {
        var v2Int = new Vector2Int((int)pos.x,(int)pos.y);
        var coin = ActiveCoins.FirstOrDefault(c => c.Position == v2Int);
        if(coin == null)
            return;
        
        var r = coin.Instance.transform;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(r.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            deathAnimParent as RectTransform,
            screenPos,
            Camera.main,
            out Vector2 localPos
        );

        GameObject uiObj = Instantiate(coinDeathAnim, deathAnimParent);
        uiObj.GetComponent<RectTransform>().anchoredPosition = localPos;

        SaveStateHandler.AddCash(1);
        textRenderer.text = $"{SaveStateHandler.GetCash()} <sprite index= 0>";
        coin.Instance.SetActive(false);
        coin.Position = Vector2.zero;
        coin.Collected = false;
        coinCollectData.collectedCashThisRound += 1;

        ActiveCoins.Remove(coin);
        Coins.Enqueue(coin);
    }

    private void Awake()
    {
        CreateCoins();
        ActiveCoins = new();
        textRenderer.text = $"{SaveStateHandler.GetCash()} <sprite index= 0>";
    }

    private void CreateCoins()
    {
        Coins = new();

        for (int i = 0; i < coinPoolCount; i++)
        {
            CoinInstance c = new CoinInstance()
            {
                Instance = Instantiate(CoinPrefab,coinsParent),
                Collected = false,
                Position = Vector2.zero
            };
            c.Instance.SetActive(false);
            Coins.Enqueue(c);
        }
    }

    private float GetSafeSpot(int y,List<Vector3Int> FreeTiles)
    {
        var validTiles = FreeTiles.Where(v => v.y == y).ToList();
        var pos = validTiles[UnityEngine.Random.Range(0,validTiles.Count)];
        FreeTiles.Remove(pos);
        return pos.x;
    }

    public List<Vector2Int> SpawnCoins(Tilemap Level)
    {
        List<Vector2Int> retTiles = new();
        List<Vector3Int> tiles = new();
        var bounds = Level.cellBounds;

        foreach (var item in bounds.allPositionsWithin)
        {
            if(Level.GetTile(item) == null)
                tiles.Add(item);
        }


        ClearCoins();
        for (int y = bounds.min.y; y < bounds.max.y - 3; y++)
        {
            if(UnityEngine.Random.Range(0,1f) > .1f)
                continue;

            var next = Coins.Dequeue();
            next.Position = new(GetSafeSpot(y,tiles),y);
            next.Instance.SetActive(true);
            retTiles.Add(new((int)next.Position.x,(int)next.Position.y));
            ActiveCoins.Add(next);
        }
        return retTiles;
    }

    private void ClearCoins()
    {
        if(ActiveCoins.Count == 0)
            return;

        foreach (var item in ActiveCoins)
        {
            item.Instance.SetActive(false);
            item.Position = Vector2.zero;
            item.Collected = false;
            Coins.Enqueue(item);
        }

        ActiveCoins.Clear();
    }
}
