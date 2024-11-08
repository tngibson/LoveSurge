using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static MusicManager instance { get; private set; }

    private EventInstance musicEventInstance;
    private List<EventInstance> eventInstances;
    private EnumSoundtrack soundtrackInstance;
    public EnumSoundtrack SoundTrackInstance { get => soundtrackInstance; set => soundtrackInstance = value; }
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Music Manager in the scene.");
            Destroy(instance);
        } 
        else
        {
           DontDestroyOnLoad(this);
        }

        instance = this;
        eventInstances = new List<EventInstance>();

    }

    private void Start()
    {
        InitializeMusic(FMODEvents.instance.gameSoundtrack);
        UnityEngine.SceneManagement.Scene scene = SceneManager.GetActiveScene();
        Debug.Log(scene);
    }
    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateInstance(musicEventReference);
        musicEventInstance.start();
    }

    public void AudioSwitcher(EnumSoundtrack track)
    {
        switch (track)
        {
            case EnumSoundtrack.SILENCE:
                musicEventInstance.setParameterByName("Song", 0);
                break;
            case EnumSoundtrack.TITLE_THEME:
                musicEventInstance.setParameterByName("Song", 1);
                Debug.Log("Title Play");
                break;
            case EnumSoundtrack.NOKI_THEME:
                musicEventInstance.setParameterByName("Song", 2);
                break;
            case EnumSoundtrack.LOTTE_THEME:
                musicEventInstance.setParameterByName("Song", 3);
                break;
            case EnumSoundtrack.ROOMATE_THEME:
                musicEventInstance.setParameterByName("Song", 4);
                break;
            case EnumSoundtrack.AVERAGE_DAY:
                musicEventInstance.setParameterByName("Song", 5);
                break;
            case EnumSoundtrack.RAND_EVENT:
                musicEventInstance.setParameterByName("Song", 6);
                break;
            case EnumSoundtrack.DEEP_CONVERSATION:
                musicEventInstance.setParameterByName("Song", 7);
                break;
        }
    }
    public void CleanUp()
    {
        //stop and release created instances
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            eventInstance.release();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
    
}
