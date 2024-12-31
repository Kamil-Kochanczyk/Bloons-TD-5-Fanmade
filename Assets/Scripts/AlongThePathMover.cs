using System.Collections;
using UnityEngine;

/*
 * TODO: AlongThePathMover description
 */

public class AlongThePathMover : MonoBehaviour
{
    private IBloon _bloon;
    private Rigidbody2D _rb;
    private float _speed;
    private int _targetPathPointIndex;
    private bool _isOnThePath;
    private bool _isGoingBackALittle;

    public int TargetPathPointIndex => _targetPathPointIndex;

    private void Awake()
    {
        _bloon = GetComponent<IBloon>();
        _rb = GetComponent<Rigidbody2D>();
        _speed = LevelManager.Instance.BloonSpeedModifier * _bloon.DefaultSpeed;
        _targetPathPointIndex = -1;
        _isOnThePath = false;
        _isGoingBackALittle = false;
    }

    private void FixedUpdate()
    {
        if (_isOnThePath && !_isGoingBackALittle)
        {
            MoveAlongThePath();
        }
    }

    private void MoveAlongThePath()
    {
        Vector2 targetPosition = PathManager.Instance.GetPathPoint(_targetPathPointIndex).position;

        _rb.MovePosition(Vector2.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime));

        if (Vector2.Distance(transform.position, targetPosition) <= _speed * Time.deltaTime)
        {
            _targetPathPointIndex++;

            if (_targetPathPointIndex > PathManager.Instance.DiePointIndex)
            {
                _targetPathPointIndex = -1;
                _isOnThePath = false;
                BloonsPoolsManager.Instance.ReturnBloon(gameObject);
                PathManager.Instance.RemoveBloonFromPath();
                LevelManager.Instance.LoseLives(_bloon.LivesWorth);
            }
        }
    }

    public void StartMovingFromSpawnPoint()
    {
        transform.position = PathManager.Instance.GetPathPoint(PathManager.Instance.SpawnPointIndex).position;
        _targetPathPointIndex = PathManager.Instance.SpawnPointIndex + 1;
        _isOnThePath = true;
    }

    public void StartMovingFromPosition(Vector2 position, int targetPathPointIndex)
    {
        transform.position = position;
        _targetPathPointIndex = targetPathPointIndex;
        _isOnThePath = true;
    }

    public void SlowDown(float slowingFactor, float seconds)
    {
        float originalSpeed = _speed;
        _speed *= slowingFactor;
        StartCoroutine(SlowDownCanceller(originalSpeed, seconds));
    }

    public void Stop(float seconds)
    {
        float originalSpeed = _speed;
        _speed = 0.0f;
        StartCoroutine(StopCanceller(originalSpeed, seconds));
    }

    public void GoBackALittle()
    {
        int newTargetPathPointIndex = _targetPathPointIndex - 5;

        if (newTargetPathPointIndex < 0)
        {
            newTargetPathPointIndex = 0;
        }

        Vector2 targetPosition = PathManager.Instance.GetRandomPositionBetweenThisAndNext(newTargetPathPointIndex);

        StartCoroutine(Regresser(targetPosition));
    }

    private IEnumerator SlowDownCanceller(float originalSpeed, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _speed /= originalSpeed;
    }

    private IEnumerator StopCanceller(float originalSpeed, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _speed = originalSpeed;
    }

    private IEnumerator Regresser(Vector2 targetPosition)
    {
        _isGoingBackALittle = true;

        while (Vector2.Distance(transform.position, targetPosition) > _speed * Time.deltaTime)
        {
            _rb.MovePosition(Vector2.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime));
            yield return null;
        }

        _isGoingBackALittle = false;
    }
}
