using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressManager : MonoBehaviour
{
    
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

    public float GetCurrentStressAmount()
    {
        return currentStressAmt;
    }

    public float AddToCurrentStress(float amount = 0.1f)
    {
        currentStressAmt += amount;
        print(currentStressAmt);
        return currentStressAmt;
    }

    public float RemoveFromCurrentStress(float amount)
    {
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
