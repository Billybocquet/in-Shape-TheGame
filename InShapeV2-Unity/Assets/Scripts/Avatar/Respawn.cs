using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Respawn : MonoBehaviour
{
    [Header("Spawner Position")]
    [SerializeField] private Transform respawnPosition;

    [Header("Limit Fall")]
    [SerializeField] private Vector3 limit;

    private void Update()
    {
        if (gameObject.transform.position.y < limit.y)
        {
            transform.position = respawnPosition.position;
        }
    }
}
