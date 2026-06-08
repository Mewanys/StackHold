using UnityEngine;
using System.Collections;

public class CloudManager : MonoBehaviour
{
    public GameObject bigCloudPrefab;

    public int islandRadius = 10;
    public float cloudHeightMin = 7f;
    public float cloudHeightMax = 12f;

    public float spawnDelayMin = 8f;
    public float spawnDelayMax = 16f;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnCloud();

            yield return new WaitForSeconds(
                Random.Range(spawnDelayMin, spawnDelayMax));
        }
    }

    void SpawnCloud()
    {
        Vector3 pos = new Vector3(
            Random.Range(-islandRadius, islandRadius),
            Random.Range(cloudHeightMin, cloudHeightMax),
            Random.Range(-islandRadius, islandRadius)
        );

        GameObject cloud = Instantiate(bigCloudPrefab, pos, Quaternion.identity);

        cloud.AddComponent<CloudBehaviour>();
    }
}

