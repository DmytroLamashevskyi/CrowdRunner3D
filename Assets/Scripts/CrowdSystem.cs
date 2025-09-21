using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdSystem : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Transform _runnersParent;  

    [Header("Settings")]
    [SerializeField] private float _radius = 2;
    [SerializeField] private float _angle = 137.508f;

    // Update is called once per frame
    void Update()
    {
        PlaceRunners();
    }

    //TODO Place Runners when some Action happens
    private void PlaceRunners()
    {
        for(int i = 0; i < _runnersParent.childCount; i++)
        {
            Vector3 childLocalPosition = GetRunnerPosition(i);
            _runnersParent.GetChild(i).localPosition = childLocalPosition;

        }
    }

    private Vector3 GetRunnerPosition(int index)
    {
        var x = _radius * Mathf.Sqrt(index) * Mathf.Cos(Mathf.Deg2Rad * index * _angle);
        var z = _radius * Mathf.Sqrt(index) * Mathf.Sin(Mathf.Deg2Rad * index * _angle);
        return new Vector3(x, 0, z);
    }


    public float GetRadius()
    {
        return _radius * Mathf.Sqrt(_runnersParent.childCount);
    }
}
