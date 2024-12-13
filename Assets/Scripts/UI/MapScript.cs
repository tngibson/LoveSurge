using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class MapScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string locInfo;
    public string locName;
    public bool useMapManager;
    public MapLocationsManager manager;

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

        if (StressBar.instance != null)
        {
            StressBar.instance.updateStressBar();
        }

        SetEnabled();
        locationText.GetComponent<TextMeshProUGUI>().text = locationTextText;
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
        if (manager != null || !useMapManager)
        {
            if (StressManager.instance != null)
            {
                StressManager.instance.addToCurrentStress();
            }
            if (StressBar.instance != null)
            {
                StressBar.instance.updateStressBar();
            }
            SceneManager.LoadScene(locName);
            gameObject.SetActive(false);

            if (useMapManager)
            {
                manager.LocationSelect(locInfo, locName);
            }

            // Check if this map item progresses the day phase
            if (CalendarManager.instance != null && isDayProgressor)
            {
                CalendarManager.instance.AdvancePhase();
            }
        }
        else
        {
            Debug.LogWarning("MapLocationsManager reference is missing on " + gameObject.name);
        }
    }

    public void SetEnabled()
    {
        if (isEnabled)
        {
            button.interactable = true;
            questionText.SetActive(false);
        }
        else
        {
            button.interactable = false;
            questionText.SetActive(true);
        }
    }

    public void ToggleEnabled()
    {
        if (isEnabled)
        {
            isEnabled = false;
            button.interactable = false;
            questionText.SetActive(true);
        }
        else
        {
            isEnabled = true;
            button.interactable = true;
            questionText.SetActive(false);
        }
    }

    public void UpdateLocationText(string newText)
    {
        locationTextText = newText;
        locationText.GetComponent<TextMeshProUGUI>().text = locationTextText;
    }
}