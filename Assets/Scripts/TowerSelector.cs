using UnityEngine;
using UnityEngine.EventSystems;

/*
 * This file allows the buttons from the canvas to work
 */

public class TowerSelector : MonoBehaviour
{
    public void SelectTower(GameObject towerPrefab)
    {
        TowerSelectionManager.Instance.SelectTowerFromUI(towerPrefab);
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
    }
}
