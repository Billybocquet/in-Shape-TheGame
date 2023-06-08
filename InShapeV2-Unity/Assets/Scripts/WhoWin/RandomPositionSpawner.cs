using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPositionSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject gameObjectPrefab;
    private RandomPositionBoundingVolume rdPosBoundingVolume;

    [Header("Text")] 
    [SerializeField] private string text;

    [Header("Script")]
    private CheckIfWin checkIfWin;
    
    // Start is called before the first frame update
    void Start()
    {
        rdPosBoundingVolume = GetComponent<RandomPositionBoundingVolume>();
        
        Vector3 rdPos = rdPosBoundingVolume.GetRandomPosition();
        GameObject checker = Instantiate(gameObjectPrefab, rdPos, Quaternion.identity);
        checkIfWin = checker.GetComponent<CheckIfWin>();
        checkIfWin.scoreText = text;
    }
}
