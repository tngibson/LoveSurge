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

    //[field: SerializeField] public EventReference gameSoundtrack { get; private set; }

   // [field: SerializeField] public EventReference dateMusic { get; private set; }
    //[field: SerializeField] public EventReference environmentTrack { get; private set; }
    [field: SerializeField] public EventReference CurrentRingtone { get; private set; }
    //[field: SerializeField] public EventReference sceneMusic { get; private set; }
    [field: Header("Card SFX")]
    [field: SerializeField] public EventReference CardPlaced { get; private set; }
    [field: SerializeField] public EventReference CardHovering { get; private set; }
    [field: SerializeField] public EventReference CardShuffle { get; private set; }
    [field: SerializeField] public EventReference CardClicked { get; private set; }
    [field: SerializeField] public EventReference DiscardCard { get; private set; }

    [field: Header("Voices")]
    [field: SerializeField] public EventReference DateVoice { get; private set; }
    [field: SerializeField] public EventReference DateVoice2 { get; private set; }
    [field: SerializeField] public EventReference PlayerVoice { get; private set; }
    [field: SerializeField] public EventReference NokiVoice { get; private set;}
    [field: SerializeField] public EventReference LotteVoice { get; private set;}
    [field: SerializeField] public EventReference CelciVoice { get; private set;}
    [field: SerializeField] public EventReference MiguelVoice { get; private set;}
    [field: SerializeField] public EventReference FishVoice { get; private set;}
    [field: SerializeField] public EventReference CeoVoice { get; private set;}
    [field: SerializeField] public EventReference WizardVoice { get; private set;}
    [field: SerializeField] public EventReference DeliahVoice { get; private set;}
    [field: SerializeField] public EventReference noVoice { get; private set;}
   
    [field: Header("UI SFX")]
    [field: SerializeField] public EventReference UiClick { get; private set; }
    [field: SerializeField] public EventReference GoodResponse { get; private set; }
    [field: SerializeField] public EventReference BadResponse { get; private set; }
    [field: SerializeField] public EventReference ConnectionBarUp { get; private set; }
    [field: SerializeField] public EventReference ConnectionBarDown { get; private set; }
    [field: SerializeField] public EventReference WindowOpen { get; private set; }
    [field: SerializeField] public EventReference WindowClose { get; private set; }


    //[field: SerializeField] public EventReference dateTierII { get; private set; }
    //[field: SerializeField] public EventReference dateTierIII { get; private set; }
    [field: SerializeField] public EventReference DiceRoll { get; private set; }
    [field: SerializeField] public EventReference DiceShake { get; private set; }

    [field: Header("Vocal SFX")]
    //[field: SerializeField] public EventReference busted { get; private set; }
    [field: SerializeField] public EventReference DatingStart { get; private set; }

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
