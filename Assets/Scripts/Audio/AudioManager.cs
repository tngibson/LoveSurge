using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Runtime.CompilerServices;
using System.Dynamic;
using FMODUnityResonance;


public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update
   public static AudioManager instance { get; private set;}
   
   private EventInstance musicEventInstance;
   private EventInstance dialougeEventInstance;
   private List<EventInstance> eventInstances;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            eventInstances = new List<EventInstance>();

        }
        else
        {
            Debug.LogWarning("AudioManager instance already exists. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    private void Start()
   {
        InitializeMusic(FMODEvents.instance.music);
        InitializeVoices(FMODEvents.instance.playerVoice);

    }

    //Play Sound Effects
    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateInstance (EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }
    
    //Start && Stop Music
    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateInstance(musicEventReference);
        musicEventInstance.start();
    }

    public void StopMusic(EventReference musicEventReference)
    {
        musicEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    //Start && Stop Dialouge
    private void InitializeVoices(EventReference eventReference)
    {
        dialougeEventInstance = CreateInstance(eventReference);
        dialougeEventInstance.start();
    }

    private void StopDialouge()
    {
        dialougeEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

    }

    public void SetDialougeType(VocalTypes vocalType)
    {
        dialougeEventInstance.setParameterByName("vocalType", (float) vocalType);
    }
    public void SetDialougeType(SilenceTrigger textTyping)
    {
        dialougeEventInstance.setParameterByName("textTyping", (float) textTyping);
    }
    private void CleanUp()
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
