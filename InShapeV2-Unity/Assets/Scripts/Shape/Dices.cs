using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dices : MonoBehaviour
{
    [SerializeField] private PhysicMaterial[] physicMaterialsList;
    [SerializeField] private float[] weightList;
    [SerializeField] private int numberOfBlocks;
    
    private Collider collider;
    private Rigidbody rigidbody;

    // Start is called before the first frame update
    void Awake()
    {
        collider = GetComponent<Collider>();
        rigidbody = GetComponent<Rigidbody>();

        int i = Random.Range(0, weightList.Length - 1);
        
        rigidbody.mass = weightList[i] * numberOfBlocks;
        collider.material = physicMaterialsList[i];
    }
}
