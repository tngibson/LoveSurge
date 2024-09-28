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
    [field: SerializeField] public EventReference music {get; private set;}

    [field: Header("Card SFX")]
    [field: SerializeField] public EventReference cardPlaced { get; private set; }
    [field: SerializeField] public EventReference cardHovering { get; private set; }
    [field: SerializeField] public EventReference cardShuffle { get; private set; }
    [field: SerializeField] public EventReference cardClicked { get; private set; }

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
