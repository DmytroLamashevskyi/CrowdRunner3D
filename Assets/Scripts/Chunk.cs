using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector3 _size;

    public float GetLength()
    {
        return _size.x;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, _size);
    }
}
