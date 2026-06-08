using UnityEngine;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    public BuildingData currentBuilding;
    public Transform buildBase;

    public int maxBlocks = 4;

    private List<float> blockAccuracy = new List<float>();
    private int placedBlocks = 0;

    void Awake()
    {
        Instance = this;
    }

    public void StartBuilding(BuildingData data)
    {
        currentBuilding = data;
        blockAccuracy.Clear();
        placedBlocks = 0;

        BlockSpawner.Instance.StartSpawning();
    }

    public void RegisterBlock(float accuracy)
    {
        blockAccuracy.Add(accuracy);
        placedBlocks++;

        if (placedBlocks >= maxBlocks)
        {
            FinishBuilding();
        }
    }

    void FinishBuilding()
    {
        BlockSpawner.Instance.StopSpawning();

        float avgAccuracy = 0;

        foreach (var a in blockAccuracy)
            avgAccuracy += a;

        avgAccuracy /= blockAccuracy.Count;

        float health = currentBuilding.baseHealth * avgAccuracy;
        float income = currentBuilding.baseIncome * avgAccuracy;
        float attack = currentBuilding.baseAttack * avgAccuracy;

        Debug.Log($"Здание построено | HP:{health} | Income:{income} | ATK:{attack}");
    }
}