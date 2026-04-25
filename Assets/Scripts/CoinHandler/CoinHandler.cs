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

    [SerializeField] AudioClip coinClip;
    
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
        SoundService.PlaySound(coinClip,0.5f,0.05f);
    }

    private void Awake()
    {
        CreateCoins();
        ActiveCoins = new();
        textRenderer.text = $"{SaveStateHandler.GetCash()} <sprite index= 0>";

        SaveStateHandler.CashChanged += (val) => textRenderer.text = $"{val} <sprite index= 0>";
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

    public List<Vector2Int> SpawnCoins(Vector3Int[] coins)
    {
        List<Vector2Int> retTiles = new();

        ClearCoins();
        foreach (var spot in coins)
        {
            var next = Coins.Dequeue();
            next.Position = new(spot.x,spot.y);
            next.Instance.SetActive(true);
            retTiles.Add(new((int)next.Position.x,(int)next.Position.y));
            ActiveCoins.Add(next);
        }
        return retTiles;
    }

    public void ClearCoins()
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
