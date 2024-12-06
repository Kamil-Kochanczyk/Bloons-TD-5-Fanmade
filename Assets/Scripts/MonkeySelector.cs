using UnityEngine;
using UnityEngine.EventSystems;

public class MonkeySelector : MonoBehaviour
{
    public void SelectMonkey(GameObject monkeyPrefab)
    {
        MonkeySelectionManager.Instance.SelectMonkeyFromUI(monkeyPrefab);
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
    }
}
