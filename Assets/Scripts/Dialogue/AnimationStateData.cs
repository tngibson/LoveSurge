using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationStateData", menuName = "Animation State Data")]
[Serializable]
public class AnimationStateData : ScriptableObject
{
    public bool RemapState; // Intended for backwards compatibility with existing portrait system
    // If you are not remapping a state, and just want to mark it as able to play silently, just put
    // the state name in both From and To, and leave RemapState unchecked.
    public string From;
    public string To;
    public bool CanPlaySilently; // Can this animation be played when the speaker isn't talking?
}
