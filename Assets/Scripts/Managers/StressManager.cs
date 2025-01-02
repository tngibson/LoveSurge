using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressManager : MonoBehaviour
{
    public static EventHandler stressFilledEvent; // Called when stress meter fills
    public static EventHandler stressUnfilledEvent; // Called when stress is above 1 and then decrements below 1
    public static StressManager instance;
    public float currentStressAmt = .1f;
    public int numStressBars = 4;

    private void Awake()
    {
        // keeps all stress values throughout the scenes
        DontDestroyOnLoad(this.gameObject);
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);  // Ensures only one instance of StressManager
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        if (currentStressAmt >= 1) stressFilledEvent?.Invoke(this, EventArgs.Empty);
    }

    public float GetCurrentStressAmount()
    {
        return currentStressAmt;
    }

    public float AddToCurrentStress(float amount = 0.1f)
    {
        // Necessary so that adding stress over 1 doesn't call the stress filled event multiple times
        if (currentStressAmt >= 1f) return currentStressAmt;

        currentStressAmt += amount;
        print(currentStressAmt);

        if (currentStressAmt >= 1f) stressFilledEvent?.Invoke(this, EventArgs.Empty);
        return currentStressAmt;
    }

    public float RemoveFromCurrentStress(float amount)
    {
        if (currentStressAmt >= 1f) stressUnfilledEvent?.Invoke(this, EventArgs.Empty);

        currentStressAmt -= amount;
        return currentStressAmt;
    }

    public static int GetStressBarsFilled(float amount)
    {
        if (amount <= 0) return 0;

        int numSteps = instance.numStressBars + 1;

        // Should round down to nearest int
        int stepIndex = (int)(amount * numSteps);

        return Math.Min(stepIndex, instance.numStressBars);
    }
}
