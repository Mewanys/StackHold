using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class IslandManager : MonoBehaviour
{
    [Header("Island")]
    public int islandRadius = 7;

    [Header("Prefabs")]
    public GameObject grassBlock;
    public GameObject pathBlock;
    public GameObject flowerBlock;
    public GameObject waterBlock;
    //public GameObject cloudBlock;
    public GameObject trampledGroundBlock;
    public GameObject bridgeBlock;

    [Header("Chances")]
    [Range(0f, 1f)] public float flowerChance = 0.18f;

    [Header("Roads")]
    [Range(2, 3)] public int roadCount = 2;
    public int roadLength = 8;

    [Header("Trampled Ground")]
    public int trampledCount = 10;

    [Header("Water")]
    public int lakeCount = 2;
    public int lakeMinRadius = 1;
    public int lakeMaxRadius = 2;
    public int riverLength = 10;

    [Header("Clouds")]
    public int cloudCount = 8;
    public float cloudHeight = 5f;

    HashSet<Vector2Int> waterTiles = new HashSet<Vector2Int>();
    HashSet<Vector2Int> pathTiles = new HashSet<Vector2Int>();

    void Start()
    {
        GenerateLakes();
        GenerateRiver();
        GenerateRoads();
        GenerateIsland();
        GenerateTrampledGround();
        //GenerateClouds();
    }

    // ================= WATER =================

    void GenerateLakes()
    {
        for (int i = 0; i < lakeCount; i++)
        {
            Vector2Int center = RandomPointInIsland();
            int radius = Random.Range(lakeMinRadius, lakeMaxRadius + 1);

            for (int x = -radius; x <= radius; x++)
                for (int z = -radius; z <= radius; z++)
                {
                    Vector2Int p = center + new Vector2Int(x, z);
                    if (Vector2.Distance(Vector2.zero, p) <= islandRadius &&
                        Vector2.Distance(p, center) <= radius)
                    {
                        waterTiles.Add(p);
                    }
                }
        }
    }

    void GenerateRiver()
    {
        Vector2Int pos = RandomPointInIsland();
        Vector2Int dir = RandomDirection();

        for (int i = 0; i < riverLength; i++)
        {
            if (Vector2.Distance(Vector2.zero, pos) > islandRadius)
                break;

            waterTiles.Add(pos);

            if (Random.value < 0.4f)
                dir = RandomDirection();

            pos += dir;
        }
    }

    // ================= ROADS =================

    void GenerateRoads()
    {
        List<Vector2Int> dirs = new List<Vector2Int>
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        for (int i = 0; i < roadCount; i++)
        {
            int index = Random.Range(0, dirs.Count);
            Vector2Int dir = dirs[index];
            dirs.RemoveAt(index);

            Vector2Int pos = Vector2Int.zero;

            for (int j = 0; j < roadLength; j++)
            {
                if (Vector2.Distance(Vector2.zero, pos) > islandRadius)
                    break;

                pathTiles.Add(pos);
                pos += dir;
            }
        }
    }

    // ================= ISLAND =================

    void GenerateIsland()
    {
        for (int x = -islandRadius; x <= islandRadius; x++)
            for (int z = -islandRadius; z <= islandRadius; z++)
            {
                Vector2Int tile = new Vector2Int(x, z);

                if (Vector2.Distance(Vector2.zero, tile) > islandRadius)
                    continue;

                Vector3 pos = new Vector3(x, 0, z);

                // Сначала вода
                if (waterTiles.Contains(tile))
                {
                    Instantiate(waterBlock, pos, Quaternion.identity, transform);
                }

                // Мостик ПОВЕРХ воды
                if (pathTiles.Contains(tile) && waterTiles.Contains(tile))
                {
                    Instantiate(
                        bridgeBlock,
                        pos + Vector3.up * 0.1f, // немного выше воды
                        Quaternion.identity,
                        transform
                    );
                }
                // Обычная тропинка
                else if (pathTiles.Contains(tile))
                {
                    Instantiate(pathBlock, pos, Quaternion.identity, transform);
                }
                // Остальное
                else if (!waterTiles.Contains(tile))
                {
                    if (Random.value < flowerChance)
                        Instantiate(flowerBlock, pos, Quaternion.identity, transform);
                    else
                        Instantiate(grassBlock, pos, Quaternion.identity, transform);
                }
            }
    }

                // ================= TRAMPLED =================

                void GenerateTrampledGround()
    {
        List<Vector2Int> candidates = new List<Vector2Int>();

        foreach (Vector2Int path in pathTiles)
        {
            candidates.Add(path + Vector2Int.left);
            candidates.Add(path + Vector2Int.right);
            candidates.Add(path + Vector2Int.up);
            candidates.Add(path + Vector2Int.down);
        }

        for (int i = 0; i < trampledCount && candidates.Count > 0; i++)
        {
            int index = Random.Range(0, candidates.Count);
            Vector2Int tile = candidates[index];
            candidates.RemoveAt(index);

            if (Vector2.Distance(Vector2.zero, tile) > islandRadius)
                continue;

            if (pathTiles.Contains(tile) || waterTiles.Contains(tile))
                continue;

            Vector3 pos = new Vector3(tile.x, 0, tile.y);
            Instantiate(trampledGroundBlock, pos, Quaternion.identity, transform);
        }
    }

    // ================= CLOUDS =================

    //void GenerateClouds()
    //{
    //    for (int i = 0; i < cloudCount; i++)
    //    {
    //        Vector3 cloudPos = new Vector3(
    //            Random.Range(-islandRadius, islandRadius),
    //            cloudHeight,
    //            Random.Range(-islandRadius, islandRadius)
    //        );

    //        Instantiate(cloudBlock, cloudPos, Quaternion.identity, transform);
    //    }
    //}

    // ================= HELPERS =================

    Vector2Int RandomPointInIsland()
    {
        Vector2Int p;
        do
        {
            p = new Vector2Int(
                Random.Range(-islandRadius, islandRadius),
                Random.Range(-islandRadius, islandRadius)
            );
        }
        while (Vector2.Distance(Vector2.zero, p) > islandRadius);

        return p;
    }

    Vector2Int RandomDirection()
    {
        switch (Random.Range(0, 4))
        {
            case 0: return Vector2Int.right;
            case 1: return Vector2Int.left;
            case 2: return Vector2Int.up;
            default: return Vector2Int.down;
        }
    }

    public void OnRestart()
    {
        SceneManager.LoadScene("Game");
    }
}