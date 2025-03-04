using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class MapScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string locName;

    [SerializeField] private Button button;
    [SerializeField] private GameObject questionText;
    [SerializeField] private GameObject locationText;
    [SerializeField] private string locationTextText;
    public bool isEnabled;

    // New boolean to check if this map item should advance the day phase
    [SerializeField] private bool isDayProgressor;

    // Scale factor and material for hover effect
    public float hoverScale = 1.1f;
    private Vector3 originalScale;

    private void Awake()
    {
        // Save original scale and material
        originalScale = transform.localScale;

        SetEnabled();
        if (locationText != null) locationText.GetComponent<TextMeshProUGUI>().text = locationTextText;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isEnabled)
        {
            // Scale up and apply hover material
            transform.localScale = originalScale * hoverScale;
            locationText.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Reset scale and material
        transform.localScale = originalScale;
        locationText.SetActive(false);
    }

    public void OnSelect()
    {
        if (StressManager.instance != null)
        {
            StressManager.instance.AddToCurrentStress();
        }
        if (StressBar.instance != null)
        {
            StressBar.instance.UpdateStressBar();
        }
        SceneManager.LoadScene(locName);
        gameObject.SetActive(false);

        // Check if this map item progresses the day phase
        if (CalendarManager.instance != null && isDayProgressor)
        {
            CalendarManager.instance.AdvancePhase();
        }
    }

    public void SetEnabled()
    {
        if (isEnabled)
        {
            if (button != null) button.interactable = true;
            if (questionText != null) questionText.SetActive(false);
        }
        else
        {
            if (button != null) button.interactable = false;
            if (questionText != null) questionText.SetActive(true);
        }
    }

    public void ToggleEnabled()
    {
        isEnabled = !isEnabled; // Toggle the state first

        if (button != null) button.interactable = isEnabled;
        if (questionText != null) questionText.SetActive(!isEnabled);
    }


    public void UpdateLocationText(string newText)
    {
        locationTextText = newText;
        locationText.GetComponent<TextMeshProUGUI>().text = locationTextText;
    }
}