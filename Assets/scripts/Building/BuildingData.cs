using UnityEngine;

[CreateAssetMenu(fileName = "NewBuildingData", menuName = "Game/Building Data")]
public class BuildingData : ScriptableObject
{
    public string buildingName;

    [Header("Block Variants")]
    [Tooltip("Массив префабов блоков для этого здания (например, 4 разных блока замка)")]
    public GameObject[] blockPrefabs;

    [Header("Building Settings")]
    public int defaultMaxBlocks = 4; // Лимит блоков, который можно будет потом увеличивать
    public string specificScriptName; // Имя скрипта, который добавится в конце (например, "ArcherTowerLogic")

    [Header("Base Stats (при идеальной стройке 100%)")]
    public float baseHealth = 1000f;
    public float baseDamage = 50f;
    public float baseProfit = 100f;
    // Сюда можно добавлять любые другие статы
}