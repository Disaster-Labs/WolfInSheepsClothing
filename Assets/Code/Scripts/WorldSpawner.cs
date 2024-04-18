// ---------------------------------------
// Creation Date: 04/09/2024
// Author: Boyi Qian
// Modified By:
// ---------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldSpawner : MonoBehaviour
{
    public Tilemap tilemap; 
    public List<GameObject> environmentPrefabs;
    private List<Vector3> grassTileworldPos = new List<Vector3>();
    private int grassTileCount;
    private int prefabCount;

    // public Tile[] resourceTiles; 
    public int minResources = 5; 
    public int maxResources = 10; 
    // public Vector3Int areaTopLeft; 
    // public Vector3Int areaBottomRight; 

    void Start()
    {
        tilemap = GetComponent<Tilemap>();

        GenerateResources();
    }

    void GenerateResources()
    {
        Vector3Int tm_origin = tilemap.origin;
        Vector3Int tm_size = tilemap.size;
        for (int x = tm_origin.x; x < tm_size.x; x++)
        {
            for (int y = tm_origin.y; y < tm_size.y; y++)
            {
                if(tilemap.GetTile(new Vector3Int(x, y,0))!= null) 
                {
                    Vector3 cellToWorldPos = tilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));
                    grassTileworldPos.Add(cellToWorldPos);

                }
            }
        }

        grassTileCount = grassTileworldPos.Count;
        prefabCount = environmentPrefabs.Count;

        int resourceCount = Random.Range(minResources, maxResources + 1);

        for (int i = 0; i < resourceCount; i++)
        {
            int aRandomTile = Random.Range(0, grassTileCount);
            Vector3 randomPosition = grassTileworldPos[aRandomTile]; 
            int aRandomRes = Random.Range(0, prefabCount);
            GameObject randomPrefab = environmentPrefabs[aRandomRes];
            Instantiate(randomPrefab, randomPosition, Quaternion.identity);
            // Tile tileToPlace = resourceTiles[Random.Range(0, resourceTiles.Length)];
            // tilemap.SetTile(randomPosition, tileToPlace);
        }
    }
}
