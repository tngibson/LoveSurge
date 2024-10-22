using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialougeChangerTrigger : MonoBehaviour
{
    [Header("Dialouge Type")]
    [SerializeField] private VocalTypes vocalType;
    [Header("Silence Type")]
    [SerializeField] private SilenceTrigger textTyping;
}
