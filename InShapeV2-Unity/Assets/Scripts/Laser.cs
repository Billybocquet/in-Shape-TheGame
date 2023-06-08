using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] Transform gunAim;
    [SerializeField] LineRenderer linieRend;
    Ray ray;
    RaycastHit hit;
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if(Physics.Raycast(ray, out hit, 100))
        {
            linieRend.enabled = true;
            linieRend.SetPosition(0, gunAim.transform.position);
            linieRend.SetPosition(1, hit.point);
            print(hit.transform.name);
        }
        else
        {
            linieRend.enabled = false;
        }
    }
}
