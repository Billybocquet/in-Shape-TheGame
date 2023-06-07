using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPositionSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject gameObjectPrefab;
    private RandomPositionBoundingVolume rdPosBoundingVolume;
    
    // Start is called before the first frame update
    void Start()
    {
        rdPosBoundingVolume = GetComponent<RandomPositionBoundingVolume>();
        
        Vector3 rdPos = rdPosBoundingVolume.GetRandomPosition();
        Instantiate(gameObjectPrefab, rdPos, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
