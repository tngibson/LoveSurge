using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressManager : MonoBehaviour
{
    public static EventHandler stressFilledEvent; // Called when stress meter fills
    public static EventHandler stressUnfilledEvent; // Called when stress is above 1 and then decrements below 1

    public static EventHandler<StressEventArgs> stressChangedEvent; // Called whenever stress changes

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

        StressBar.instance?.UpdateStressBar();
    }

    private void Start()
    {
        // Invoke events on start to make sure things are updated at the beginning of the game
        if (currentStressAmt >= 1) stressFilledEvent?.Invoke(this, EventArgs.Empty);
        stressChangedEvent?.Invoke(this, new StressEventArgs(){AmountChanged = 0, NewTotal = currentStressAmt});
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
        stressChangedEvent?.Invoke(this, new StressEventArgs()
        {
            AmountChanged = amount,
            NewTotal = currentStressAmt
        });

        if (currentStressAmt >= 1f) stressFilledEvent?.Invoke(this, EventArgs.Empty);
        StressBar.instance?.UpdateStressBar();
        return currentStressAmt;
    }

    public float RemoveFromCurrentStress(float amount)
    {
        if (currentStressAmt >= 1f && amount > 0) stressUnfilledEvent?.Invoke(this, EventArgs.Empty);
        if (currentStressAmt - amount < 0) amount = currentStressAmt; // Set amount so stress will equal 0

        currentStressAmt -= amount;

        stressChangedEvent?.Invoke(this, new StressEventArgs()
        {
            AmountChanged = -amount,
            NewTotal = currentStressAmt
        });

        if (StressBar.instance != null)
        {
            StressBar.instance.UpdateStressBar();
        }
        else
        {
            Debug.LogWarning("StressBar instance is null! Skipping UpdateStressBar() to prevent errors.");
        }

        return currentStressAmt;
    }

    // Gets the number of bars filled rounded down
    public static int GetStressBarsFilled(float amount)
    {
        if (amount <= 0) return 0;

        int numSteps = instance.numStressBars;

        // Should round down to nearest int
        int stepIndex = (int)(amount * numSteps);

        return Math.Min(stepIndex, instance.numStressBars);
    }
}

public class StressEventArgs : EventArgs
{
    public float AmountChanged;
    public float NewTotal;
}
