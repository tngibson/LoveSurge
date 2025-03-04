using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class SoundtrackSetter : MonoBehaviour
{
    [field: SerializeField] public EnumSoundtrack track { get; private set; }
    private void Start()
    {
        if (MusicManager.instance != null)
        {
            MusicManager.instance.AudioSwitcher(track);
        }
        else
        {
            Debug.LogWarning("Music Manager was null!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
