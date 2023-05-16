using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomSpawner : MonoBehaviour
{
    [Header("Shape")]
    [SerializeField] private GameObject[] shapePrefabList;
    
    [Header("Shape Count")]
    [SerializeField] private float spawnCount;
    
    [Header("Spawn Range")]
    [SerializeField] private float xRange;
    [SerializeField] private float yRange;
    [SerializeField] private float zRange;

    // Start is called before the first frame update
    void Start() 
    {
        for (int i = 0; i < spawnCount; i++)
        {
            int randomIndex = Random.Range(0, shapePrefabList.Length);

            Vector3 randomeSpawnPosition = new Vector3(Random.Range(-xRange, xRange), yRange, Random.Range(-zRange, zRange));
            Instantiate(shapePrefabList[randomIndex], randomeSpawnPosition, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
