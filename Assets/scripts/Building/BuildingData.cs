using UnityEngine;

[CreateAssetMenu(menuName = "CityBloxx/Building")]
public class BuildingData : ScriptableObject
{
    public string buildingName;

    public float baseHealth;
    public float baseIncome;
    public float baseAttack;
}

