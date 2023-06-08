using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPositionBoundingVolume : MonoBehaviour
{
    private new Collider collider;

    private void Awake()
    {
        collider = GetComponent<Collider>();
    }

    public Vector3 GetRandomPosition()
    {
        Bounds bounds = collider.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector3(x, y, z);
    }
}
