using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomSpawner : MonoBehaviour
{
    [Header("Shape")]
    [SerializeField] private GameObject[] shapePrefabList;

    [Header("ShapeSpawn")] 
    [SerializeField] private GameObject[] shapeGameObjectsSpawned;
    
    [Header("Shape Count")]
    [SerializeField,Range(0, 100)] private int spawnCount;
    
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

            Vector3 randomeSpawnPosition = new Vector3(Random.Range(-xRange, xRange) + gameObject.transform.position.x, yRange, Random.Range(-zRange, zRange) + gameObject.transform.position.z);
            Instantiate(shapePrefabList[randomIndex], randomeSpawnPosition, Quaternion.identity);
            
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
