using System;
using UnityEngine;

public class AlongThePathMover : MonoBehaviour
{
    private string _pathGameObjectName = "path";
    private Transform[] _path = null;
    private int _targetPathPointIndex = -1;
    private const float _speed = 1.0f;

    private void Start()
    {
        try
        {
            SetPath();
            transform.position = _path[0].position;
            _targetPathPointIndex = 1;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private void Update()
    {
        MoveAlongThePath();
    }

    private void SetPath()
    {
        GameObject pathGameObject = GameObject.Find(_pathGameObjectName);

        if (pathGameObject == null)
        {
            throw new Exception($"SetPath: GameObject with name \"{_pathGameObjectName}\" doesn't exist.");
        }

        Transform path = pathGameObject.transform;

        int childCount = path.childCount;

        if (childCount < 2)
        {
            throw new Exception($"SetPath: GameObject with name \"{_pathGameObjectName}\" has to have at least two children.");
        }

        _path = new Transform[childCount];

        for (int i = 0; i < childCount; i++)
        {
            _path[i] = path.GetChild(i);
        }
    }

    private void MoveAlongThePath()
    {
        transform.position = Vector2.MoveTowards(transform.position, _path[_targetPathPointIndex].position, _speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, _path[_targetPathPointIndex].position) <= _speed * Time.deltaTime)
        {
            _targetPathPointIndex++;

            if (_targetPathPointIndex >= _path.Length)
            {
                Destroy(gameObject);
            }
        }
    }
}
