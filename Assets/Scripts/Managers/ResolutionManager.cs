using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ResolutionManager : MonoBehaviour
{
    [Header("Dropdowns")]
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown screenModeDropdown;

    [Header("Confirmation UI")]
    public GameObject confirmPanel; // Panel with "Keep / Revert"
    public TMP_Text countdownText;
    public float confirmTime = 10f;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions = new List<Resolution>();

    private int pendingResolutionIndex;
    private int pendingScreenModeIndex;

    private int appliedResolutionIndex;
    private int appliedScreenModeIndex;

    private Coroutine revertCoroutine;

    void Start()
    {
        InitializeResolutions();
        InitializeScreenModes();
        LoadSettings();
    }

    // -------------------------
    // INITIALIZATION
    // -------------------------
    void InitializeResolutions()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();
        filteredResolutions.Clear();

        HashSet<string> unique = new HashSet<string>();
        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string label = resolutions[i].width + " x " + resolutions[i].height;

            if (unique.Contains(label))
                continue;

            unique.Add(label);
            filteredResolutions.Add(resolutions[i]);
            options.Add(label);
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
    }

    void InitializeScreenModes()
    {
        screenModeDropdown.ClearOptions();

        List<string> options = new List<string>
        {
            "Fullscreen",
            "Borderless",
            "Windowed"
        };

        screenModeDropdown.AddOptions(options);
        screenModeDropdown.onValueChanged.AddListener(OnScreenModeChanged);
    }

    // -------------------------
    // PENDING CHANGES
    // -------------------------
    void OnResolutionChanged(int index)
    {
        pendingResolutionIndex = index;
    }

    void OnScreenModeChanged(int index)
    {
        pendingScreenModeIndex = index;
    }

    // -------------------------
    // APPLY SETTINGS
    // -------------------------
    public void ApplySettings()
    {
        ApplyResolution(pendingResolutionIndex, pendingScreenModeIndex);

        // Show confirmation UI
        confirmPanel.SetActive(true);

        if (revertCoroutine != null)
            StopCoroutine(revertCoroutine);

        revertCoroutine = StartCoroutine(RevertCountdown());
    }

    void ApplyResolution(int resIndex, int modeIndex)
    {
        Resolution res = filteredResolutions[resIndex];
        FullScreenMode mode = GetScreenMode(modeIndex);

        Screen.fullScreenMode = mode;
        Screen.SetResolution(res.width, res.height, mode);
    }

    // -------------------------
    // CONFIRM / REVERT
    // -------------------------
    IEnumerator RevertCountdown()
    {
        float timer = confirmTime;

        while (timer > 0)
        {
            timer -= Time.unscaledDeltaTime;

            // Update countdown text (rounded up so it feels natural)
            if (countdownText != null)
            {
                int seconds = Mathf.CeilToInt(timer);
                countdownText.text = "Reverting in " + seconds + "...";
            }

            yield return null;
        }

        RevertSettings();
    }

    public void ConfirmSettings()
    {
        if (revertCoroutine != null)
            StopCoroutine(revertCoroutine);

        appliedResolutionIndex = pendingResolutionIndex;
        appliedScreenModeIndex = pendingScreenModeIndex;

        SaveSettings();

        confirmPanel.SetActive(false);

        if (countdownText != null)
            countdownText.text = "";
    }

    public void RevertSettings()
    {
        if (revertCoroutine != null)
            StopCoroutine(revertCoroutine);

        ApplyResolution(appliedResolutionIndex, appliedScreenModeIndex);

        resolutionDropdown.value = appliedResolutionIndex;
        screenModeDropdown.value = appliedScreenModeIndex;

        resolutionDropdown.RefreshShownValue();
        screenModeDropdown.RefreshShownValue();

        confirmPanel.SetActive(false);

        if (countdownText != null)
            countdownText.text = "";
    }

    // -------------------------
    // LOAD / SAVE
    // -------------------------
    void LoadSettings()
    {
        bool hasSaved = PlayerPrefs.HasKey("ResolutionIndex");

        if (hasSaved)
        {
            appliedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
            appliedScreenModeIndex = PlayerPrefs.GetInt("ScreenMode", 1);
        }
        else
        {
            // Detect native resolution
            Resolution native = Screen.currentResolution;

            appliedResolutionIndex = FindMatchingResolutionIndex(native);
            appliedScreenModeIndex = 1; // Default to Bordered Fullscreen
        }

        appliedResolutionIndex = Mathf.Clamp(appliedResolutionIndex, 0, filteredResolutions.Count - 1);
        appliedScreenModeIndex = Mathf.Clamp(appliedScreenModeIndex, 0, 2);

        pendingResolutionIndex = appliedResolutionIndex;
        pendingScreenModeIndex = appliedScreenModeIndex;

        resolutionDropdown.value = appliedResolutionIndex;
        screenModeDropdown.value = appliedScreenModeIndex;

        resolutionDropdown.RefreshShownValue();
        screenModeDropdown.RefreshShownValue();

        ApplyResolution(appliedResolutionIndex, appliedScreenModeIndex);
    }

    int FindMatchingResolutionIndex(Resolution target)
    {
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            if (filteredResolutions[i].width == target.width &&
                filteredResolutions[i].height == target.height)
            {
                return i;
            }
        }

        // Fallback if exact match not found
        return filteredResolutions.Count - 1;
    }

    void SaveSettings()
    {
        PlayerPrefs.SetInt("ResolutionIndex", appliedResolutionIndex);
        PlayerPrefs.SetInt("ScreenMode", appliedScreenModeIndex);
        PlayerPrefs.Save();
    }

    // -------------------------
    // UTIL
    // -------------------------
    FullScreenMode GetScreenMode(int index)
    {
        switch (index)
        {
            case 0: return FullScreenMode.ExclusiveFullScreen;
            case 1: return FullScreenMode.FullScreenWindow;
            case 2: return FullScreenMode.Windowed;
            default: return FullScreenMode.FullScreenWindow;
        }
    }
}