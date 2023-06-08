using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInteractUI : MonoBehaviour
{
    [Header("Container")]
    [SerializeField] private GameObject containerGameObject;
    [SerializeField] private GameObject containerGameObject2;
    
    [Header("Script")]
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private GravityGun3 gravityGun3;
    
    [Header("TextMeshPro")]
    [SerializeField] private TextMeshProUGUI interactTextMeshProUGUI;

    private void Update()
    {
        if (playerInteract.GetSpawnerInteractable() != null)
        {
            Show(playerInteract.GetSpawnerInteractable());
        }
        else
        {
            Hide(containerGameObject);
        }

        if (gravityGun3.IsEditing == true)
        {
            Show(containerGameObject2);
        }
        else
        {
            Hide(containerGameObject2);
        }
    }

    private void Show(ShapeSpawner shapeSpawner)
    {
        containerGameObject.SetActive(true);
        interactTextMeshProUGUI.text = shapeSpawner.GetInteractText();
    }
    
    private void Show(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }

    private void Hide(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
}
