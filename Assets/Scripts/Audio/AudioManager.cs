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

   //UI Volume Control
   [Header("Volume")]
   [Range(0, 1)]
   public float masterVolume = 1;
   [Range(0, 1)]
   public float musicVolume = 1;
   [Range(0, 1)]
   public float sfxVolume = 1;
   [Range(0, 1)]

   private Bus masterBus;
   private Bus musicBus;
   private Bus sfxBus;

   private EventInstance musicEventInstance;
   private EventInstance dialougeEventInstance;
   private List<EventInstance> eventInstances;

   private void Awake()
   {
    if (instance != null)
    {
        Debug.LogError("Found more than one Audio Manager in the scene.");
    }
        instance = this;
        eventInstances = new List<EventInstance>();
        Object.Destroy(this);

        //Set Bus
        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");

    }

    private void Start()
   {
        //InitializeMusic(FMODEvents.instance.music);
        InitializeVoices(FMODEvents.instance.playerVoice);

    }

    private void Update()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        sfxBus.setVolume(sfxVolume);
    }
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
    
    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateInstance(musicEventReference);
        musicEventInstance.start();
    }

    private void InitializeVoices(EventReference eventReference)
    {
        dialougeEventInstance = CreateInstance(eventReference);
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
