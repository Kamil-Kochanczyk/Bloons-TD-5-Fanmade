using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSelector : MonoBehaviour
{
    public void SelectTower(GameObject towerPrefab)
    {
        TowerSelectionManager.Instance.SelectTowerFromUI(towerPrefab);
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
    }
}
