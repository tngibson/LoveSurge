using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressManager : MonoBehaviour
{
    
    public static StressManager instance;
    public float currentStressAmt = .1f;

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

    public float AddToCurrentStress()
    {
        currentStressAmt += .1f;
        print(currentStressAmt);
        return currentStressAmt;
    }

    public float RemoveFromCurrentStress(float amount)
    {
        currentStressAmt -= amount;
        return currentStressAmt;
    }
}
