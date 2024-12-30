using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private string speakerName = "Default";

    private static EventHandler<AnimatorEventData> startAnimationEvent;
    private static EventHandler<AnimatorEventData> stopAnimationEvent;

    private AnimationStateData[] spriteToAnimationMap;
    private string currentState;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        // Load only animation data for this character
        spriteToAnimationMap = Resources.LoadAll<AnimationStateData>("AnimationData/" + speakerName);

        animator = GetComponent<Animator>();

        startAnimationEvent += OnStartAnimation;
        stopAnimationEvent += OnStopAnimation;
    }

    void OnDestroy()
    {
        startAnimationEvent -= OnStartAnimation;
        stopAnimationEvent -= OnStopAnimation;
    }

    // Could probably be optimized to avoid searching the entire list every time
    private (bool, string) HasStateMapping(AnimationStateData[] map, string fromState)
    {
        foreach (var s2a in map)
        {
            if (s2a.RemapState && s2a.From == fromState) return (true, s2a.To);
        }

        return (false, "");
    }

    private bool CanAnimationPlaySilently(AnimationStateData[] map, string fromState)
    {
        foreach (var s2a in map)
        {
            if (s2a.From == fromState && s2a.CanPlaySilently) return true;
        }

        return false;
    }

    private void OnStartAnimation(object sender, AnimatorEventData args)
    {
        string state = args.State;
        // Replace the animation state to transition to if we have a mapping from
        // the given state to some other state (intended for backwards compatibility with
        // existing systems)
        (bool res, string to) = HasStateMapping(spriteToAnimationMap, state);
        if (res) state = to;

        // Move to the new animation state
        Debug.Log("Moving to state " + state);
        animator.enabled = true;

        if ((args.Speaker == speakerName || CanAnimationPlaySilently(spriteToAnimationMap, args.State)) 
            && animator.HasState(0, Animator.StringToHash(state)))
        {
            // Character is actually speaking (or a silent animation should play), play the animation (if it exists)
            animator.CrossFade(Animator.StringToHash(state), 0.0f, 0);
            currentState = state;
        }
        else
        {
            // Character is not speaking or no animation exists for this state, so use the existing sprite
            animator.CrossFade(Animator.StringToHash("No_Animation"), 0.0f, 0);
            currentState = "";
            animator.enabled = false;
        }
    }

    private void OnStopAnimation(object sender, AnimatorEventData args)
    {
        if (currentState != "" && (args.Speaker == speakerName || CanAnimationPlaySilently(spriteToAnimationMap, args.State)))
        {
            string state = currentState + "_Idle";
            // Might as well allow you to specify which state to go to based on the current state
            (bool res, string to) = HasStateMapping(spriteToAnimationMap, currentState);
            if (res) state = to;

            Debug.Log("Moving to state " + state);
            animator.CrossFade(Animator.StringToHash(state), 0.0f, 0);
            currentState = state;
        }
    }

    // Use these anywhere to start/stop animations
    // Note that ALL CharacterAnimators will receive this, so you will need
    // to use the speakerName property to filter who actually starts animating
    public static void InvokeStartAnimation(object sender, AnimatorEventData args)
    {
        startAnimationEvent.Invoke(sender, args);
    }

    public static void InvokeStopAnimation(object sender, AnimatorEventData args)
    {
        stopAnimationEvent.Invoke(sender, args);
    }
}

public class AnimatorEventData : EventArgs
{
    public string State;
    public string Speaker;
}
