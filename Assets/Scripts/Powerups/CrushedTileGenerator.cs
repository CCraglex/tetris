
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CrushedTileGenerator : MonoBehaviour
{
    [SerializeField] private GameObject tileObjPrefab;
    [SerializeField] private Transform parentForGenTiles;

    private void Awake() {}

    public void CreateFakeTileAt(Tilemap map,Vector3Int tilePos)
    {
        var newFake = Instantiate(tileObjPrefab,parentForGenTiles);
        newFake.transform.position = map.CellToWorld(tilePos);
        var sr = newFake.GetComponent<SpriteRenderer>();
        sr.sprite = map.GetSprite(tilePos);
        StartCoroutine(Destruction(sr));
    }

    private IEnumerator Destruction(SpriteRenderer sr)
    {
        float dt = 0.25f;
        float t = 0;

        while(t < dt)
        {
            yield return null;
            t += Time.deltaTime;
            
            sr.color = new(1,1,1,t/dt);
        }

        Destroy(sr.gameObject);
    }
}