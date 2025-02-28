using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using FMODUnity;
using System;
using Unity.VisualScripting;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Music")]

    [field: SerializeField] public EventReference gameSoundtrack { get; private set; }

    [field: SerializeField] public EventReference dateMusic { get; private set; }
    [field: SerializeField] public EventReference environmentTrack { get; private set; }
    [field: SerializeField] public EventReference envIntroSound { get; private set; }
    [field: SerializeField] public EventReference currentRingtone { get; private set; }


    //[field: SerializeField] public EventReference music { get; private set; }
    // [field: SerializeField] public EventReference lotteTheme {get; private set;}
    //[field: SerializeField] public EventReference samTheme { get; private set; }
    //[field: SerializeField] public EventReference roomateTheme { get; private set; }
    //[field: SerializeField] public EventReference deepConversation { get; private set; }
    //[field: SerializeField] public EventReference anAverageDay { get; private set; }



    [field: Header("Card SFX")]
    [field: SerializeField] public EventReference cardPlaced { get; private set; }
    [field: SerializeField] public EventReference cardHovering { get; private set; }
    [field: SerializeField] public EventReference cardShuffle { get; private set; }
    [field: SerializeField] public EventReference cardClicked { get; private set; }

    [field: Header("Voices")]
    [field: SerializeField] public EventReference dateVoice { get; private set; }
    [field: SerializeField] public EventReference dateVoice2 { get; private set; }
    [field: SerializeField] public EventReference playerVoice { get; private set; }

    [field: Header("UI SFX")]
    [field: SerializeField] public EventReference uiClick { get; private set; }
    [field: SerializeField] public EventReference goodResponse { get; private set; }
    [field: SerializeField] public EventReference badResponse { get; private set; }
    [field: SerializeField] public EventReference connectionBarUp { get; private set; }
    [field: SerializeField] public EventReference connectionBarDown { get; private set; }


    [field: SerializeField] public EventReference dateTierII { get; private set; }
    [field: SerializeField] public EventReference dateTierIII { get; private set; }
    [field: SerializeField] public EventReference diceRoll { get; private set; }
    [field: SerializeField] public EventReference diceShake { get; private set; }

    [field: Header("Vocal SFX")]
    [field: SerializeField] public EventReference busted { get; private set; }
    [field: SerializeField] public EventReference datingStart { get; private set; }

    public static FMODEvents instance { get; private set; }
   


    public void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in this scene");
        }
        instance = this;
    }
}
