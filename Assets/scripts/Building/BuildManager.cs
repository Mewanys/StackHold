using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    [Header("Cameras")]
    public Camera mainCamera;
    public Camera buildCamera;

    [Header("Build Setup")]
    public Transform craneTransform;
    public Transform startPlatform;
    public Transform finalDestination;

    [Header("UI")]
    public GameObject statsWindow;
    public Text statsText;

    [Header("Sway Settings")]
    public float swaySpeed = 2f;
    public float swayDistance = 3f;

    [Header("Animation Settings")]
    [Tooltip("Аниматор")]
    public Animator animator;
    public string idleAnim = "idle";
    public string releaseAnim = "release";

    private BuildingData currentBuilding;
    private int currentMaxBlocks;

    private GameObject currentSpawningBlock;
    private BlockBehaviour currentBlockBehaviour;

    // Список блоков, которые СЕЙЧАС стоят на башне
    private List<BlockBehaviour> placedBlocks = new List<BlockBehaviour>();

    // Хранилище точности для каждого конкретного блока
    private Dictionary<BlockBehaviour, float> blockAccuracies = new Dictionary<BlockBehaviour, float>();

    private bool isBuilding = false;
    private float spawnHeightOffset = 1f;

    void Update()
    {
        if (!isBuilding) return;

        // Качание крана
        float sway = Mathf.Sin(Time.time * swaySpeed) * swayDistance;
        craneTransform.position = new Vector3(startPlatform.position.x + sway, craneTransform.position.y, startPlatform.position.z);

        // Сброс блока
        if (Input.GetMouseButtonDown(0) && currentSpawningBlock != null)
        {
            currentBlockBehaviour.Drop();
            currentSpawningBlock = null; // Освобождаем переменную, ждем приземления
        }
    }

    public void StartBuilding(BuildingData buildingData)
    {
        currentBuilding = buildingData;
        currentMaxBlocks = buildingData.defaultMaxBlocks;

        placedBlocks.Clear();
        blockAccuracies.Clear(); // Очищаем данные точности

        isBuilding = true;

        mainCamera.gameObject.SetActive(false);
        buildCamera.gameObject.SetActive(true);

        statsWindow.SetActive(false);
        craneTransform.position = new Vector3(startPlatform.position.x, startPlatform.position.y + 5f, startPlatform.position.z);

        SpawnNextBlock();
    }

    private void SpawnNextBlock()
    {
        // Проверяем лимит строго перед созданием нового блока
        if (placedBlocks.Count >= currentMaxBlocks)
        {
            FinishBuilding();
            return;
        }

        // Вычисляем высоту крана
        UpdateCraneHeight();

        GameObject randomPrefab = currentBuilding.blockPrefabs[Random.Range(0, currentBuilding.blockPrefabs.Length)];

        currentSpawningBlock = Instantiate(randomPrefab, craneTransform.position, Quaternion.identity, craneTransform);
        currentBlockBehaviour = currentSpawningBlock.GetComponent<BlockBehaviour>();

        currentBlockBehaviour.OnBlockLanded += HandleBlockLanded;
        currentBlockBehaviour.OnBlockFell += HandleBlockFell;
    }

    // Метод для динамического выравнивания крана по высоте башни
    private void UpdateCraneHeight()
    {
        float currentHeight = placedBlocks.Count > 0 ? placedBlocks[placedBlocks.Count - 1].transform.position.y : startPlatform.position.y;
        craneTransform.position = new Vector3(craneTransform.position.x, currentHeight + spawnHeightOffset, craneTransform.position.z);
    }

    private void HandleBlockLanded(BlockBehaviour block)
    {
        block.OnBlockLanded -= HandleBlockLanded;

        placedBlocks.Add(block);

        // Считаем точность для ЭТОГО блока
        Transform targetTransform = placedBlocks.Count > 1 ? placedBlocks[placedBlocks.Count - 2].transform : startPlatform;
        float distance = Vector2.Distance(
            new Vector2(block.transform.position.x, block.transform.position.z),
            new Vector2(targetTransform.position.x, targetTransform.position.z)
        );

        float accuracyMultiplier = Mathf.Clamp01(1f - (distance / 1.5f));

        // Записываем точность блока в словарь
        blockAccuracies[block] = accuracyMultiplier;

        // Фиксация предпоследнего блока
        if (placedBlocks.Count >= 2)
        {
            placedBlocks[placedBlocks.Count - 2].Freeze();
        }

        // Спавним следующий блок (он становится дочерним к крану)
        SpawnNextBlock();
    }

    private void HandleBlockFell(BlockBehaviour block)
    {
        block.OnBlockLanded -= HandleBlockLanded;
        block.OnBlockFell -= HandleBlockFell;

        // Проверяем: этот блок упал после того как коснулся (соскользнул)?
        if (placedBlocks.Contains(block))
        {
            placedBlocks.Remove(block);

            // Удаляем его точность из расчетов, чтобы не завышать статы
            if (blockAccuracies.ContainsKey(block))
            {
                blockAccuracies.Remove(block);
            }

            Debug.Log("Блок соскользнул! Удален из расчетов.");

            // ВАЖНО: Новый блок НА КРАНЕ уже создался в HandleBlockLanded.
            // Мы НЕ вызываем SpawnNextBlock(), а просто опускаем кран ниже, 
            // так как башня уменьшилась. Блок на кране опустится вместе с ним автоматически!
            UpdateCraneHeight();
        }
        else
        {
            // Это был чистый промах, на кране пусто. Вот теперь спавним новый блок.
            Debug.Log("Чистый промах!");
            SpawnNextBlock();
        }
    }

    private void FinishBuilding()
    {
        isBuilding = false;

        // Считаем финальную среднюю точность на основе оставшихся блоков в словаре
        float totalAccuracy = 0f;
        foreach (var acc in blockAccuracies.Values)
        {
            totalAccuracy += acc;
        }

        float averageAccuracy = placedBlocks.Count > 0 ? totalAccuracy / placedBlocks.Count : 0f;

        int finalHealth = Mathf.RoundToInt(currentBuilding.baseHealth * averageAccuracy);
        int finalDamage = Mathf.RoundToInt(currentBuilding.baseDamage * averageAccuracy);
        int finalProfit = Mathf.RoundToInt(currentBuilding.baseProfit * averageAccuracy);

        GameObject finishedBuilding = new GameObject(currentBuilding.buildingName + "_Completed");
        finishedBuilding.transform.position = finalDestination.position;
        finishedBuilding.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        foreach (var block in placedBlocks)
        {
            block.Freeze();
            block.transform.SetParent(finishedBuilding.transform);
        }

        BuildingStats stats = finishedBuilding.AddComponent<BuildingStats>();
        stats.Health = finalHealth;
        stats.Damage = finalDamage;
        stats.Profit = finalProfit;

        if (!string.IsNullOrEmpty(currentBuilding.specificScriptName))
        {
            System.Type scriptType = System.Type.GetType(currentBuilding.specificScriptName);
            if (scriptType != null)
            {
                finishedBuilding.AddComponent(scriptType);
            }
        }

        statsWindow.SetActive(true);
        statsText.text = $"Стройка завершена!\n\n" + //$"Успешных блоков: {placedBlocks.Count} / {currentMaxBlocks}\n" +
                         $"Точность: {Mathf.RoundToInt(averageAccuracy * 100)}%\n" +
                         $"Здоровье: {finalHealth}\n" +
                         $"Урон: {finalDamage}\n" +
                         $"Прибыль: {finalProfit}";

        Invoke(nameof(ResetCameras), 5f);
    }

    private void ResetCameras()
    {
        statsWindow.SetActive(false);
        buildCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
    }
}