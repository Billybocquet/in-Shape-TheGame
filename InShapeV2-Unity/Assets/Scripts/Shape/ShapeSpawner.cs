using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using FMODUnity;
using Random = UnityEngine.Random;

public class ShapeSpawner : MonoBehaviour
{
    [Header("Shape")] 
    [SerializeField] private GroupOfShapeList[] ShapeGroupLists;
    [SerializeField] private GameObject[] ShapePrefabList;
    
    [Header("Probas")]
    [SerializeField, Range(0, 100)] private float[] probasMaterial;
    [SerializeField, Range(0, 100)] private float[] probasShape;
    

    [Header("Shape Cooldown")]
    [SerializeField,Range(0, 100)] private float spawnCooldown;
    private float timer;

    [Header("Spawn Point")] 
    [SerializeField] private Transform spawnPoint;

    [Header("Interact")] 
    [SerializeField] private string interactText;

    public string soundEvent;
    private void Start()
    {
        timer = 0;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
    }

    public void Spawn()
    {
        if (timer <= 0)
        {
            int randomMaterialIndex = RandomWeightedID(probasMaterial);

            for (int i = 0; i < ShapeGroupLists[randomMaterialIndex].shapeList.Length; i++)
            {
                ShapePrefabList[i] = ShapeGroupLists[randomMaterialIndex].shapeList[i];
            }
        
            int randomShapeIndex = RandomWeightedID(probasShape);

            Vector3 randomSpawnPosition = spawnPoint.position;
            Instantiate(ShapePrefabList[randomShapeIndex], randomSpawnPosition, Quaternion.identity);
            RuntimeManager.PlayOneShot(soundEvent);

            timer = spawnCooldown;
        }
    }

    private int RandomWeightedID(float[] probs)
    {
        float total = 0f;
        foreach (var prob in probs)
        {
            total += prob;
        }

        float result = Random.value * total;

        for (int i = 0; i < probs.Length; i++)
        {
            if (result < probs[i])
            {
                return i;
            }
            else
            {
                result -= probs[i];
            }
        }
    
        return probs.Length - 1;
    }

    public string GetInteractText()
    {
        return interactText;
    }
}