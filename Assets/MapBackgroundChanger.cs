using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundChanger : MonoBehaviour
{
    [SerializeField] private Image backgroundImage; // UI Image component for the background
    [SerializeField] private Sprite morningSprite;  // Sprite for Morning
    [SerializeField] private Sprite eveningSprite;  // Sprite for Evening
    [SerializeField] private Sprite nightSprite;    // Sprite for Night

    private void Start()
    {
        UpdateBackground(CalendarManager.instance.currentPhase); // Set the initial background
    }

    private void OnEnable()
    {
        CalendarManager.instance.OnPhaseChanged += UpdateBackground;
    }

    private void OnDisable()
    {
        CalendarManager.instance.OnPhaseChanged -= UpdateBackground;
    }

    private void UpdateBackground(DayPhase phase)
    {
        switch (phase)
        {
            case DayPhase.Morning:
                backgroundImage.sprite = morningSprite;
                break;
            case DayPhase.Evening:
                backgroundImage.sprite = eveningSprite;
                break;
            case DayPhase.Night:
                backgroundImage.sprite = nightSprite;
                break;
            default:
                Debug.LogWarning("Unhandled DayPhase: " + phase);
                break;
        }
    }
}