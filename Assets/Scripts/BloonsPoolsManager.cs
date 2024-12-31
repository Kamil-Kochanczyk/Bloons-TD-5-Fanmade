using System.Collections.Generic;
using UnityEngine;

/*
 * TODO: BloonsPoolsManager description
 */

public class BloonsPoolsManager : MonoBehaviour
{
    // The order in the arrays must match the order in the enum BloonType

    [SerializeField] private GameObject[] _bloonsPrefabs;

    private Queue<GameObject>[] _bloonsPools;

    private List<GameObject> _neededBloons;

    public static BloonsPoolsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        int bloonsTypesCount = System.Enum.GetValues(typeof(BloonType)).Length;
        _bloonsPools = new Queue<GameObject>[bloonsTypesCount];
        for (int i = 0; i < _bloonsPools.Length; i++)
        {
            _bloonsPools[i] = new Queue<GameObject>();
        }

        _neededBloons = new List<GameObject>();
    }

    public GameObject GetBloon(BloonType bloonType)
    {
        int bloonTypeIndex = (int)bloonType;
        GameObject bloon = null;

        if (_bloonsPools[bloonTypeIndex].Count != 0)
        {
            bloon = _bloonsPools[bloonTypeIndex].Dequeue();
            bloon.SetActive(true);
        }
        else
        {
            bloon = Instantiate(_bloonsPrefabs[bloonTypeIndex]);
        }

        return bloon;
    }

    public void ReturnBloon(GameObject bloon)
    {
        bloon.SetActive(false);
        _bloonsPools[(int)bloon.GetComponent<IBloon>().BloonType].Enqueue(bloon);
    }

    public void InitializeNewBloonsPools(RoundWave roundWave)
    {
        _neededBloons.Clear();

        foreach (BloonWave bloonWave in roundWave.bloonWaves)
        {
            for (int i = 0; i < bloonWave.bloonsCount; i++)
            {
                RepopulateRecursively(bloonWave.bloonType);
            }
        }

        foreach (GameObject neededBloon in _neededBloons)
        {
            ReturnBloon(neededBloon);
        }
    }

    private void RepopulateRecursively(BloonType bloonType)
    {
        GameObject bloon = GetBloon(bloonType);
        _neededBloons.Add(bloon);
        IBloon iBloon = bloon.GetComponent<IBloon>();

        for (int i = 0; i < iBloon.ChildrenCount; i++)
        {
            RepopulateRecursively(iBloon.ChildType);
        }
    }

    public void ClearAllPools()
    {
        int bloonsTypesCount = System.Enum.GetValues(typeof(BloonType)).Length;

        for (int i = 0; i < bloonsTypesCount; i++)
        {
            while (_bloonsPools[i].Count != 0)
            {
                Destroy(GetBloon((BloonType)i));
            }
        }
    }
}
