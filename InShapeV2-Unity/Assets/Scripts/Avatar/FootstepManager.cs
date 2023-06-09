using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepManager : MonoBehaviour
{
    private FMOD.Studio.EventInstance footsteps;
    
    public void PlayFootstep()
    {
        footsteps = FMODUnity.RuntimeManager.CreateInstance(("event:/Players/Walk"));
        footsteps.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        footsteps.start();
        footsteps.release();
        //UnityEngine.Debug.Log("lol!");
    }
    
    void Update()
    {
        
    }
}
