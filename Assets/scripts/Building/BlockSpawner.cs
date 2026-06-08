using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public static BlockSpawner Instance;

    public GameObject fallingBlockPrefab;
    public Transform spawnPoint;

    private bool spawning = false;

    void Awake()
    {
        Instance = this;
    }

    public void StartSpawning()
    {
        spawning = true;
        SpawnBlock();
    }

    public void StopSpawning()
    {
        spawning = false;
    }

    public void SpawnBlock()
    {
        if (!spawning) return;

        Instantiate(fallingBlockPrefab, spawnPoint.position, Quaternion.identity);
    }
}