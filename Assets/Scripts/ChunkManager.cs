using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [Header("Elements")]

    [SerializeField] private Chunk[] _chunks;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 chunkPosition = Vector3.zero;

        for(int i = 0; i < _chunks.Length; i++)
        {
            int chunkIndex = Random.Range(0, _chunks.Length);

            if(i > 0)
            {
                chunkPosition.z += _chunks[chunkIndex].GetLength() / 2;
            }

            Chunk instance = Instantiate(_chunks[chunkIndex], chunkPosition, Quaternion.identity, transform);

            chunkPosition.z += instance.GetLength()/2;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
