using UnityEngine;

/*
 * TODO: PathManager description
 */

public class PathManager : MonoBehaviour
{
    // Path must contain at least two path points - spawn point and die point

    [SerializeField] private GameObject _path;
    private float[] _distancesFromSpawnPoint;

    public int SpawnPointIndex => 0;
    public int DiePointIndex => _path.transform.childCount - 1;

    public uint BloonsCurrentlyOnPath { get; private set; }

    public static PathManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        float accumulatedDistance = 0.0f;
        _distancesFromSpawnPoint = new float[_path.transform.childCount];
        _distancesFromSpawnPoint[SpawnPointIndex] = 0.0f;
        for (int i = 1; i < _distancesFromSpawnPoint.Length; i++)
        {
            float distanceToPreviousPathPoint = Vector2.Distance(GetPathPoint(i).position, GetPathPoint(i - 1).position);
            accumulatedDistance += distanceToPreviousPathPoint;
            _distancesFromSpawnPoint[i] = accumulatedDistance;
        }

        BloonsCurrentlyOnPath = 0;
    }

    public Transform GetPathPoint(int index)
    {
        if (index >= SpawnPointIndex && index <= DiePointIndex)
        {
            return _path.transform.GetChild(index);
        }
        else
        {
            return null;
        }
    }

    public float GetDistanceFromSpawnPoint(GameObject bloon)
    {
        AlongThePathMover alongThePathMover = bloon.GetComponent<AlongThePathMover>();
        Transform previousPathPoint = GetPathPoint(alongThePathMover.TargetPathPointIndex - 1);
        return _distancesFromSpawnPoint[alongThePathMover.TargetPathPointIndex - 1] + Vector2.Distance(previousPathPoint.position, alongThePathMover.transform.position);
    }

    public Vector2 GetRandomPositionBetweenThisAndNext(int thisPathPointIndex)
    {
        if (thisPathPointIndex >= SpawnPointIndex && thisPathPointIndex <= DiePointIndex - 1)
        {
            int nextPathPointIndex = thisPathPointIndex + 1;
            Transform thisPathPoint = GetPathPoint(thisPathPointIndex);
            Transform nextPathPoint = GetPathPoint(nextPathPointIndex);
            float t = Random.Range(0.0f, 0.5f);
            return Vector2.Lerp(thisPathPoint.position, nextPathPoint.position, t);
        }

        return GetPathPoint(SpawnPointIndex).position;
    }

    public Vector2 GetShiftedPositionAlongThePath(GameObject bloon, float shift)
    {
        AlongThePathMover alongThePathMover = bloon.GetComponent<AlongThePathMover>();
        int targetPathPointIndex = alongThePathMover.TargetPathPointIndex;
        Vector2 shiftedPosition = bloon.transform.position;

        if (shift > 0.0f)
        {
            Vector2 forwardDirection = (GetPathPoint(targetPathPointIndex).position - bloon.transform.position).normalized;
            shiftedPosition += (shift * forwardDirection);
        }
        else
        {
            Vector2 backwardDirection = (bloon.transform.position - GetPathPoint(targetPathPointIndex - 1).position).normalized;
            shiftedPosition += (-shift * backwardDirection);
        }

        return shiftedPosition;
    }

    public void AddBloonToPath()
    {
        BloonsCurrentlyOnPath++;
    }

    public void RemoveBloonFromPath()
    {
        if (BloonsCurrentlyOnPath > 0)
        {
            BloonsCurrentlyOnPath--;

            if (BloonsCurrentlyOnPath == 0 && RoundWaveManager.Instance.IsRoundWaveUnleashed)
            {
                LevelManager.Instance.EndRound();
            }
        }
    }
}
