using System.Linq;
using UnityEngine;

/*
 * TODO: RangeController description
 */

public class RangeController : MonoBehaviour
{
    [SerializeField] private LayerMask _bloonLayerMask;
    private ITower _tower;

    private void Awake()
    {
        _tower = transform.parent.GetComponent<ITower>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_tower.IsPlacedOnMap)
        {
            if (other.gameObject.layer == 7)
            {
                FindNewTarget();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_tower.IsPlacedOnMap)
        {
            if (other.gameObject.layer == 7)
            {
                FindNewTarget();
            }
        }
    }

    public Collider2D[] GetBloonsWithinRange()
    {
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        return Physics2D.OverlapCircleAll((Vector2)transform.position + collider.offset, collider.radius * transform.lossyScale.x, _bloonLayerMask);
    }

    public void FindNewTarget()
    {
        Collider2D[] bloonsWithinRange = GetBloonsWithinRange();

        if (!_tower.CanSeeCamo)
        {
            bloonsWithinRange = bloonsWithinRange.ToList().Where(bloon => !bloon.GetComponent<IBloon>().IsCamo).ToArray();
        }

        GameObject newTarget = null;

        if (bloonsWithinRange.Length > 0)
        {
            switch (_tower.TargetPriority)
            {
                case TargetPriority.First:
                    newTarget = SetNewFirstestTarget(bloonsWithinRange);
                    break;
                case TargetPriority.Last:
                    newTarget = SetNewLastestTarget(bloonsWithinRange);
                    break;
                case TargetPriority.Close:
                    newTarget = SetNewClosestTarget(bloonsWithinRange);
                    break;
                case TargetPriority.Strong:
                    newTarget = SetNewStrongestTarget(bloonsWithinRange);
                    break;
            }
        }

        _tower.SetNewTarget(newTarget);
    }

    private GameObject SetNewFirstestTarget(Collider2D[] bloonsWithinRange)
    {
        GameObject firstest = bloonsWithinRange[0].gameObject;
        float firstestDistance = PathManager.Instance.GetDistanceFromSpawnPoint(firstest);

        for (int i = 1; i < bloonsWithinRange.Length; i++)
        {
            float distance = PathManager.Instance.GetDistanceFromSpawnPoint(bloonsWithinRange[i].gameObject);

            if (distance > firstestDistance)
            {
                firstest = bloonsWithinRange[i].gameObject;
                firstestDistance = distance;
            }
        }

        return firstest;
    }

    private GameObject SetNewLastestTarget(Collider2D[] bloonsWithinRange)
    {
        GameObject lastest = bloonsWithinRange[0].gameObject;
        float lastestDistance = PathManager.Instance.GetDistanceFromSpawnPoint(lastest);

        for (int i = 1; i < bloonsWithinRange.Length; i++)
        {
            float distance = PathManager.Instance.GetDistanceFromSpawnPoint(bloonsWithinRange[i].gameObject);

            if (distance < lastestDistance)
            {
                lastest = bloonsWithinRange[i].gameObject;
                lastestDistance = distance;
            }
        }

        return lastest;
    }

    private GameObject SetNewClosestTarget(Collider2D[] bloonsWithinRange)
    {
        Transform _towerTransform = transform.parent.transform;

        GameObject closest = bloonsWithinRange[0].gameObject;
        float closestDistance = Vector2.Distance(_towerTransform.position, bloonsWithinRange[0].transform.position);

        for (int i = 1; i < bloonsWithinRange.Length; i++)
        {
            float distance = Vector2.Distance(_towerTransform.position, bloonsWithinRange[i].transform.position);

            if (distance < closestDistance)
            {
                closest = bloonsWithinRange[i].gameObject;
                closestDistance = distance;
            }
        }

        return closest;
    }

    private GameObject SetNewStrongestTarget(Collider2D[] bloonsWithinRange)
    {
        GameObject strongest = bloonsWithinRange[0].gameObject;
        int strongestRank = (int)strongest.GetComponent<IBloon>().BloonType;

        for (int i = 1; i < bloonsWithinRange.Length; i++)
        {
            int rank = (int)bloonsWithinRange[i].GetComponent<IBloon>().BloonType;

            if (rank > strongestRank)
            {
                strongest = bloonsWithinRange[i].gameObject;
                strongestRank = rank;
            }
        }

        return strongest;
    }
}
