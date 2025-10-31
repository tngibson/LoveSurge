using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using FMODUnity;

public class MapScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string locName;

    [SerializeField] private Button button;
    [SerializeField] private GameObject questionText;
    [SerializeField] private GameObject locationTextPanel;
    [SerializeField] private GameObject locationText;
    [SerializeField] private string locationTextText;
    public bool isEnabled;

    [SerializeField] private Animator mapAnimator;

    [SerializeField] private Animator transition;
    [SerializeField] private float transitionTime;

    // New boolean to check if this map item should advance the day phase
    [SerializeField] private bool isDayProgressor;

    [SerializeField] private bool isDateButton;

    // Scale factor and material for hover effect
    public float xHoverScale = 1;
    public float yHoverScale = 1;

    public float xRaycastPadding;
    public float yRaycastPadding;
    public float zRaycastPadding;
    public float wRaycastPadding;
    private Vector4 originalPadding;
    private Vector4 raycastPadding;

    private Vector3 originalScale;
    private Vector3 hoverScale;

    [SerializeField] private bool isStressReducer;
    [SerializeField] private bool isStressIncreaser;

    [SerializeField] private EventReference nextSceneMusic;

    [SerializeField] public bool hasLocationText;

    private void Awake()
    {
        // Save original scale and material
        originalScale = transform.localScale;
        originalPadding = GetComponent<Image>().raycastPadding;
        hoverScale = new Vector3 (xHoverScale, yHoverScale);
        raycastPadding = new Vector4 (xRaycastPadding, yRaycastPadding, zRaycastPadding, wRaycastPadding);

        SetEnabled(isEnabled);

        if (locationText != null) locationText.GetComponent<TextMeshProUGUI>().text = locationTextText;
    }

    void OnEnable()
    {
        if (LocationManager.Instance != null)
        {
            LocationManager.Instance.TryBindMapScript(this);
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isEnabled)
        {
            // Scale up and apply hover material
            transform.localScale = hoverScale;

            //GetComponent<Image>().raycastPadding = raycastPadding;

            if (hasLocationText)
            {
                locationTextPanel.SetActive(true);
            }

            if (mapAnimator != null)
            {
                mapAnimator.SetBool("isHovered", true);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isEnabled)
        {
            // Reset scale and material
            transform.localScale = originalScale;
            GetComponent<Image>().raycastPadding = originalPadding;

            if (hasLocationText)
            {
                locationTextPanel.SetActive(false);
            }

            if (mapAnimator != null)
            {
                mapAnimator.SetBool("isHovered", false);
            }
        }
    }

    public void OnSelect()
    {
        //Debug.Log(isStressReducer);
        
        if (locName == "Quit")
        {
            RestartApplication();
            return;
        }

        if (StressManager.instance != null && isStressReducer)
        {
            Debug.Log("Reducing Stress!");
            StressManager.instance.RemoveFromCurrentStress(0.1f);
        }
        if (StressManager.instance != null && isStressIncreaser)
        {
            Debug.Log("Adding Stress!");
            StressManager.instance.AddToCurrentStress();
        }

        if (StressBar.instance != null)
        {
            StressBar.instance.UpdateStressBar();
        }

        if (isDateButton)
        {
            string charName = locName.StartsWith("Noki") ? "Noki" :
                              locName.StartsWith("Celci") ? "Celci" :
                              locName.StartsWith("Lotte") ? "Lotte" : "";

            if (!string.IsNullOrEmpty(charName))
            {
                if (LocationManager.Instance != null && CalendarManager.instance != null)
                {
                    LocationManager.Instance.SetPhaseEnteredDate(charName, CalendarManager.instance.currentPhase);
                }
            }
        }


        StartCoroutine(LoadScene(locName));

        // Check if this map item progresses the day phase
        if (CalendarManager.instance != null && isDayProgressor)
        {
            CalendarManager.instance.AdvancePhase();
        }
    }

    IEnumerator LoadScene(string scene)
    {
        // Check if there is a scene transition
        // If there is, transition before loading the scene
        if (transition != null)
        {
            transition.SetTrigger("Start");
            //Check next Scene music
            if (MusicManager.Instance != null && MusicManager.Instance.WillChangeTo(nextSceneMusic))
                {
                    MusicManager.Instance.StopMusic();
                    Debug.Log("Stopping music before loading new scene.");
                }
            yield return new WaitForSeconds(transitionTime);
        }
        
        if (MusicManager.Instance != null && MusicManager.Instance.WillChangeTo(nextSceneMusic))
                {
                    MusicManager.Instance.StopMusic();
                    Debug.Log("Stopping music before loading new scene.");
                }

        yield return null; // wait one frame so the UI event finishes

        // Loads scene
        SceneManager.LoadScene(scene);
        gameObject.SetActive(false);
    }

    private void RestartApplication()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in the editor
    #else
        string exePath = System.IO.Path.Combine(Application.dataPath, "../LoveSurge.exe"); // Adjust path to point to the .exe
        System.Diagnostics.Process.Start(exePath); // Relaunch the game
        Application.Quit(); // Close the current instance
    #endif
    }

    public void SetEnabled(bool state)
    {
        if (state)
        {
            if (isEnabled)
            {
                if (button != null) button.interactable = true;
                if (questionText != null) questionText.SetActive(false);
            }
        }
        else
        {
            if (!isEnabled)
            {
                if (button != null) button.interactable = false;
                if (questionText != null) questionText.SetActive(true);
            }
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