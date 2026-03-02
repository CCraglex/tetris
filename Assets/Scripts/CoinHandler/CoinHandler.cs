using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class CoinInstance
{
    public GameObject Instance;
    public bool Collected;
    public Vector2 Position;
}

public class CoinHandler : MonoBehaviour
{
    [SerializeField] private GameObject CoinPrefab;
    [SerializeField] int coinPoolCount;
    [SerializeField] Transform coinsParent;

    private CoinInstance[] Coins;
    private List<CoinInstance> ActiveCoins;


    public static CoinHandler GetCoinHandler()
        => FindFirstObjectByType<CoinHandler>();
    
    public bool IsHittingACoin(Vector2 PlayerPos)
    {
        foreach (var Coin in ActiveCoins)
        {
            if((PlayerPos - Coin.Position).magnitude < 0.1f && Coin.Collected == false)
            {
                SaveStateHandler.AddCash(1);
                Coin.Collected = true;
                return true;
            }
        }

        return false;
    }

    private void Awake()
    {
        CreateCoins();
    }

    private void CreateCoins()
    {
        Coins = new CoinInstance[coinPoolCount];

        for (int i = 0; i < coinPoolCount; i++)
        {
            Coins[i] = new CoinInstance()
            {
                Instance = Instantiate(CoinPrefab,coinsParent),
                Collected = false,
                Position = Vector2.zero
            };

            Coins[i].Instance.SetActive(false);
        }
    }

    private Vector2 GetSafeSpot(List<Vector3Int> FreeTiles)
    {
        var pos = FreeTiles[Random.Range(0,FreeTiles.Count)];
        FreeTiles.Remove(pos);

        return new(pos.x,pos.y);
    }

    public void SpawnCoins(int Amount,Tilemap Level)
    {
        List<Vector3Int> tiles = new();
        var bounds = Level.cellBounds;

        foreach (var item in bounds.allPositionsWithin)
        {
            if(Level.GetTile(item) == null)
                tiles.Add(item);
        }

        if(Amount > coinPoolCount)
            Amount = coinPoolCount;

        ClearCoins();
        for (int i = 0; i < Amount; i++)
        {
            Coins[i].Position = GetSafeSpot(tiles);
            Coins[i].Instance.SetActive(true);
        }
    }

    private void ClearCoins()
    {
        foreach (var item in ActiveCoins)
        {
            item.Instance.SetActive(false);
            item.Position = Vector2.zero;
            item.Collected = false;
        }

        ActiveCoins.Clear();
    }
}
