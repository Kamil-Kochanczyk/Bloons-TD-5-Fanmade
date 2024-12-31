using System.Collections.Generic;
using UnityEngine;

/*
 * TODO: ItemsPoolsManager description
 */

public class ItemsPoolsManager : MonoBehaviour
{
    // The order in the arrays must match the order in the enum ItemType

    [SerializeField] private GameObject[] _itemsPrefabs;

    private Queue<GameObject>[] _itemsPools;

    public static ItemsPoolsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        int itemsTypesCount = System.Enum.GetValues(typeof(ItemType)).Length;
        _itemsPools = new Queue<GameObject>[itemsTypesCount];
        for (int i = 0; i < _itemsPools.Length; i++)
        {
            _itemsPools[i] = new Queue<GameObject>();
        }
    }

    public GameObject GetItem(ItemType itemType)
    {
        int itemTypeIndex = (int)itemType;
        GameObject item = null;

        if (_itemsPools[itemTypeIndex].Count != 0)
        {
            item = _itemsPools[itemTypeIndex].Dequeue();
            item.SetActive(true);
        }
        else
        {
            item = Instantiate(_itemsPrefabs[itemTypeIndex]);
        }

        return item;
    }

    public void ReturnItem(GameObject item)
    {
        item.SetActive(false);
        _itemsPools[(int)item.GetComponent<IItem>().ItemType].Enqueue(item);
    }
}
