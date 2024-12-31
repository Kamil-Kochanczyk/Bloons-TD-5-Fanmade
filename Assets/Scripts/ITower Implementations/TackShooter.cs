using System.Collections;
using UnityEngine;

public class TackShooter : MonoBehaviour, ITower
{
    public TowerType TowerType => TowerType.TackShooter;
    public ItemType UsedItem => ItemType.TackShooterTacks;
    public float BreakTime => 0.75f;
    public GameObject BloonTarget => _bloonTarget;
    public bool IsPlacedOnMap => _isPlacedOnMap;
    public TargetPriority TargetPriority => _targetPriority;
    public uint KilledBloons => _killedBloons;
    public bool CanSeeCamo => _canSeeCamo;

    private GameObject _bloonTarget = null;
    private bool _isPlacedOnMap = false;
    private TargetPriority _targetPriority = TargetPriority.First;
    private uint _killedBloons = 0;
    private bool _canSeeCamo = false;

    private void Start()
    {
        StartCoroutine(Behave());
    }

    public IEnumerator Behave()
    {
        while (true)
        {
            transform.GetChild(0).GetComponent<RangeController>().FindNewTarget();

            if (LevelManager.Instance.IsRoundOngoing && IsPlacedOnMap && BloonTarget != null)
            {
                GameObject tacks = ItemsPoolsManager.Instance.GetItem(UsedItem);
                tacks.transform.position = transform.position;

                IItem iItem = tacks.GetComponent<IItem>();
                iItem.SetNewOwner(gameObject);
                iItem.PerformAction();

                yield return new WaitForSeconds(BreakTime);
            }
            else
            {
                yield return null;
            }
        }
    }

    public void SetNewTarget(GameObject newTarget)
    {
        _bloonTarget = newTarget;
    }

    public void PlaceOnMap()
    {
        _isPlacedOnMap = true;
    }

    public void SetNewTargetPriority(TargetPriority newTargetPriority)
    {
        _targetPriority = newTargetPriority;
    }

    public void IncreaseKilledBloons(uint killedBloons)
    {
        _killedBloons += killedBloons;
    }

    public void EnableToSeeCamo()
    {
        _canSeeCamo = true;
    }
}
