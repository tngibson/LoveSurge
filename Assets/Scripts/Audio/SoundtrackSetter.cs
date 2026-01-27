using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using FMODUnity;
using UnityEditor;
using UnityEngine;

public class SoundtrackSetter : MonoBehaviour
{
    [Tooltip("Set Music Here!")]
    [SerializeField] private EventReference _musicReference;
    [SerializeField] private string _musicName;

    [SerializeField] private string DateCharacter;

    public void Awake()
    {
        
    }

    public void PlayMusicByReference(EventReference musicReference)
    {
        try
        {
            MusicManager.Instance.PlayMusic(musicReference);
            _musicReference = musicReference;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error playing music: " + e.Message);   
        }
    }

    public void PlayMusic()
    {
        //Debug.Log(MusicManager.Instance);
        if (_musicReference.IsNull)
        {
            Debug.LogWarning("Music reference is null");
            return;
        }   
        _musicName = MusicManager.GetEventName(_musicReference);
        if(!MusicManager.Instance.ActiveMusicName.Equals(_musicReference))
        {
            //Debug.Log("Playing music: " + _musicName );
            MusicManager.Instance?.PlayMusic(_musicReference);
        }  
    }

    private void Start()
    {
        PlayMusic();


        if (DateCharacter == "Noki")
        {
            MusicManager.SetParameterByName("dateCharacter", 2);
        }
        
        if (DateCharacter == "Lotte")
        {
            MusicManager.SetParameterByName("dateCharacter", 1);
        }
        
        if (DateCharacter == "Celci")
        {
            MusicManager.SetParameterByName("dateCharacter", 0);
        }
        
        if (DateCharacter == "")
        {
            MusicManager.SetParameterByName("dateCharacter", 3);
        }
    }
}

#if UNITY_EDITOR
public class MusicPlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space(10);

        SoundtrackSetter soundtrackSetter = (SoundtrackSetter)target;
        if(GUILayout.Button("Play New Music"))
        {
            soundtrackSetter.PlayMusic();
        }
    }
}
#endif
