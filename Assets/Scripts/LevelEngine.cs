
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class LevelEngine : MonoBehaviour
{
    private Tilemap Map;

    public Tilemap GetTilemap()
    {
        Map = GetComponent<Tilemap>();
        return Map;
    }
    
    public List<Vector3Int> GetSpotsOfTiles(string name)
    {
        var retVal = new List<Vector3Int>();
        var Bounds = Map.cellBounds;

        foreach (var item in Bounds.allPositionsWithin)
        {
            var tile = Map.GetTile(item);
            if(tile != null && tile.name == name)
                retVal.Add(new Vector3Int(item.x,item.y,0));
        }

        return retVal;
    }

    public TileBase GetTileFromName(string Name)
    {
        var Bounds = Map.cellBounds;
        foreach (var item in Bounds.allPositionsWithin)
        {
            var tile = Map.GetTile(item);
            if(tile == null)
                continue;
                
            if(tile.name == Name)
                return tile;
        }

        print("No tile found!");
        return null;
    }
}