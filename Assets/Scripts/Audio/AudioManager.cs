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
    public static AudioManager instance { get; private set; }

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
    private EventInstance environmentEventInstance;
    private EventInstance dialougeEventInstance;
    private EventInstance dateMusicInstance;
    private List<EventInstance> eventInstances;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);   
        }
        eventInstances = new List<EventInstance>();

        //Set Bus
        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
    }


    private void Start()
    {
        InitializeVoices(FMODEvents.instance.PlayerVoice);
        InitializeEnvironment(FMODEvents.instance.EnvIntroSound);
        AudioSceneCheck.instance?.isMatch();
    }

    private void Update()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        sfxBus.setVolume(sfxVolume);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos = default)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateInstance (EventReference eventReference)
    {
       // if (eventReference.IsNull) { return EventInstance; }
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }
/**
    private void InitializeMusic(EventReference musicEventReference)
    {
        if(musicEventReference.IsNull) { return; }
        musicEventInstance = CreateInstance(musicEventReference);
        musicEventInstance.start();
    }

    private void InitializeDate(EventReference dateMusic)
    {
        dateMusicInstance = CreateInstance(dateMusic);
        dateMusicInstance.start();
    }

    public void DateProgress(EnumDateProgress track)
    {
        switch (track)
        {
            case EnumDateProgress.CARD_DATE:
                dateMusicInstance.setParameterByName("dateProgress", 0);
                break;
            case EnumDateProgress.SKILL_CHECK:
                dateMusicInstance.setParameterByName("dateProgress", 1);
                break;
            case EnumDateProgress.START:
                dateMusicInstance.start();
                break;
            case EnumDateProgress.STOP:
                dateMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                break;
            case EnumDateProgress.GOOD_END:
                 dateMusicInstance.setParameterByName("dateProgress", 2);
                break;
            case EnumDateProgress.BAD_END:
                dateMusicInstance.setParameterByName("dateProgress", 3);
                break;

        }
    }

    public void DateCharacter(EnumDateCharacter character)
    {
        switch (character)
        {
            case EnumDateCharacter.CELCI:
                dateMusicInstance.setParameterByName("dateCharacter", 0);
                break;
            case EnumDateCharacter.LOTTE:
                dateMusicInstance.setParameterByName("dateCharacter", 1);
                break;
            case EnumDateCharacter.NOKI:
                dateMusicInstance.setParameterByName("dateCharacter", 2);
                break;
        }
    }
**/

    private void InitializeEnvironment(EventReference musicEventReference)
    {
        //environmentEventInstance = CreateInstance(musicEventReference);
        PlayOneShot(FMODEvents.instance.EnvIntroSound, this.transform.position);
        //environmentEventInstance.start();
    }

    private void InitializeVoices(EventReference eventReference)
    {
        dialougeEventInstance = CreateInstance(eventReference);
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
    public void CleanUpLoop(EventInstance eventInstance)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            eventInstance.release();
     }

    private void OnDestroy()
    {
        CleanUp();
    }

    public void SetPaused(bool isPaused)
    {
        masterBus.setPaused(isPaused);
    }
}
