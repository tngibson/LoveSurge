using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using TMPro;
using System.IO;
using UnityEditor;
using UnityEditor.Rendering;

public class MusicManager : MonoBehaviour
{
    #region Serialzied Fields
    public static MusicManager Instance;
    [SerializeField] private string currentMusicName;

    [SerializeField] private EventReference
        titleTheme,
        mapTheme,
        nokiTheme,
        lotteTheme,
        celciTheme,
        roomateTheme,
        randomEventTheme,
        quietTheme,
        loveTheme,
        dateMusic,
        deepConversation,
        creepyTheme;

    [SerializeField] private bool canSetParameter = false;
        
    #region FMOD Paramaters

    [SerializeField] private int dateCharacter = 0;
    [SerializeField] private int dateProgress = 0;

    #endregion
    
    #endregion

    #region Properties

    public EventInstance CurrentMusicInstance {get; private set;}
    public string ActiveMusicName {get => currentMusicName; set => currentMusicName = value; }
    public EventReference TitleTheme { get => titleTheme; }
    public EventReference MapTheme { get => mapTheme; }
    public EventReference NokiTheme { get => nokiTheme; }
    public EventReference LotteTheme { get => lotteTheme; }
    public EventReference CelciTheme { get => celciTheme; }
    public EventReference RandomEvent { get => randomEventTheme; }
    public EventReference QuietTheme { get => quietTheme; }
    public EventReference DateMusic { get => dateMusic; }
    public EventReference LoveTheme { get => loveTheme; }
    public EventReference CreepyTheme { get => creepyTheme; }
    public EventReference DeepConversation { get => deepConversation; }
    public Coroutine TransitionCoroutine { get; private set; }
    #endregion
   
    #region Music Manager
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);   
        }
    }

    public void Reset()
    {
        Instance.CurrentMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        ActiveMusicName = string.Empty;
    }

    //Get or set the current music instance paramater given the current music's name
    private void Update()
    {
        if (ActiveMusicName.Equals("DateMusic"))
        {
            CurrentMusicInstance.getParameterByName("dateProgress", out float dateProgress);
            CurrentMusicInstance.getParameterByName("dateCharacter", out float dateCharacter);
            dateProgress = (int)dateProgress;
            dateCharacter = (int)dateCharacter;

            if(!canSetParameter)
                return;
            SetParameterByName("dateProgress", dateProgress);
            SetParameterByName("dateCharacter", dateCharacter); 
        }
    }
    public bool WillChangeTo(EventReference newMusic)
    {
        if (!CurrentMusicInstance.isValid()) 
            return true;

       string newMusicPath = GetEventName(newMusic);
    //    Debug.Log(newMusicPath);
    //    Debug.Log(ActiveMusicName);
       return !ActiveMusicName.Equals(newMusicPath, StringComparison.OrdinalIgnoreCase);
    }
    public void PlayMusic(EventReference music, bool fadeout = false, float fadeTime = 2f)
    {
        string newMusicPath = GetEventName(music);

        if (CurrentMusicInstance.isValid() && ActiveMusicName.Equals(newMusicPath, StringComparison.OrdinalIgnoreCase))
        {
            //Debug.Log($"MusicManager:" + newMusicPath + "is already playing, not starting again.");
            return;
        }

        //Handles Null -> no song
        if(!CurrentMusicInstance.isValid())
        {
            CurrentMusicInstance = RuntimeManager.CreateInstance(music);
            CurrentMusicInstance.start();
            
            ActiveMusicName = GetEventName(music);
            return;
        }

        //Handles if there is a song
        if(!fadeout) 
        {
            CurrentMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            CurrentMusicInstance = RuntimeManager.CreateInstance(music);
            CurrentMusicInstance.start();
            ActiveMusicName = GetEventName(music);
        }
        else
        {
            StartCoroutine(FadeMusicInOut(fadeTime, music));
        }
        //print("MusicManager - Played Music: " + Instance.ActiveMusicName);
    }

    public void StopMusic()
    {
        try
        {
            CurrentMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            ActiveMusicName = string.Empty;
        }
        catch(SystemException)
        {
            Debug.LogWarning("MusicManager: StopMusic failed to stop " + Instance.CurrentMusicInstance);
        }
    }

    public static void SetParameterByName(string parameter, float value)
    {
        try
        {
            //Debug.Log("Setting " + parameter + " to " + value);
            Instance.CurrentMusicInstance.setParameterByName(parameter, value);
            //Debug.Log(Instance.CurrentMusicInstance.getParameterByName(parameter, out float paraValue));
        }
        catch (System.Exception)
        {
            Debug.LogWarning("MusicManager: SetParameter failed to set " + parameter + "to " + value);
        }
    }

   public static void SetParameterByLabel(string parameter, string label)
    {
         Instance.CurrentMusicInstance.setParameterByNameWithLabel(parameter, label);
    }

    private IEnumerator FadeMusicInOut(float delay, EventReference newMusic)
    {
        CurrentMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        yield return new WaitForSeconds(delay);

        string newMusicName = GetEventName(newMusic);
        if (ActiveMusicName.Equals(newMusicName, StringComparison.OrdinalIgnoreCase))
            yield break;

        CurrentMusicInstance = RuntimeManager.CreateInstance(newMusic);
        CurrentMusicInstance.start();
        
        #if UNITY_EDITOR
         ActiveMusicName = GetMusicName(newMusic);
        #endif
    }

    public static string GetEventName(string path)
    {
        if (string.IsNullOrEmpty(path)) 
            return "";
        int lastSlash = path.LastIndexOf('/');
        return lastSlash >= 0 ? path.Substring(lastSlash + 1) : path;
    }

    public static string GetEventName(EventReference music)
    {
        if(string.IsNullOrEmpty(music.ToString()))
            return string.Empty;

        return GetEventName(music.ToString());
    }

#if UNITY_EDITOR
        private static string GetMusicName(EventReference music)
        {
        if (string.IsNullOrEmpty(GetEventName(music))) 
            return string.Empty;

        return Path.GetFileName(GetEventName(music));
        }
#endif
}
    #endregion
    #region Designer Tool    

#if UNITY_EDITOR

[CustomEditor(typeof(MusicManager))]
public class MusicManagerEditor : Editor
{
    SerializedProperty
        CanSetParameter,
        DateProgress,
        DateCharacter;

    bool DateGroup;
        private void OnEnable()
        {
            CanSetParameter = serializedObject.FindProperty("canSetParameter");

            DateProgress = serializedObject.FindProperty("dateProgress");
            DateCharacter = serializedObject.FindProperty("dateCharacter");
        }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
            
        serializedObject.Update();
            
        MusicManager musicManager = (MusicManager)target;
            
        EditorGUILayout.PropertyField(CanSetParameter);
            
        if (musicManager.ActiveMusicName.Equals("DateTheme"))
        {
            if (DateGroup = EditorGUILayout.BeginFoldoutHeaderGroup(DateGroup, "Date Parameters"))
            {
                ShowDateParameters();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    
    serializedObject.ApplyModifiedProperties();

    }

    private void OnDisable()
    {
        serializedObject.ApplyModifiedProperties();
    }

    private void ShowDateParameters()
    {
        EditorGUILayout.LabelField("Date Parameters", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(DateProgress);
        EditorGUILayout.PropertyField(DateCharacter);
    }
}    
 #endif
 #endregion