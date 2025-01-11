using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class SoundtrackSetter : MonoBehaviour
{
    [field: SerializeField] public EnumSoundtrack track { get; private set; }
    private void Start()
    {
        MusicManager.instance.AudioSwitcher(track);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
