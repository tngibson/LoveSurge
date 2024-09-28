using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;


public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update
   public static AudioManager instance { get; private set;}
   

   private void Awake()
   {
    if (instance != null)
    {
        Debug.LogError("Found more than one Audio Manager in the scene.");
    }
    instance = this;
   }
   
   public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }
}
