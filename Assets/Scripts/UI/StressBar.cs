using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StressBar : MonoBehaviour
{
    public static StressBar instance;

    [Header("Stress Settings")]
    public float currentStressAmt;

    [Header("UI References")]
    [SerializeField] private Image StressBarFill; // The Image with 'Filled' type

    private void Awake()
    {
        // Singleton setup — ensures only one StressBar exists
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        // Initialize from StressManager
        currentStressAmt = StressManager.instance.GetCurrentStressAmount();
        UpdateStressBar();
    }

    public void UpdateStressBar()
    {
        // Always grab latest stress amount
        currentStressAmt = StressManager.instance.GetCurrentStressAmount();

        // Make sure the fill image exists
        if (StressBarFill != null)
        {
            // Clamp to ensure it stays between 0 and 1
            StressBarFill.fillAmount = Mathf.Clamp01(currentStressAmt);
        }
    }
}

