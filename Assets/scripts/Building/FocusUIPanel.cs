using UnityEngine;

public class FocusUIPanel : MonoBehaviour
{
    public BuildingData selectedBuilding;

    public void BuildSelected()
    {
        BuildingManager.Instance.StartBuilding(selectedBuilding);
    }
}