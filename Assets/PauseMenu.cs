using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject audioMenuUI;
    [SerializeField] private GameObject optionsMenuUI;
    public static PauseMenu instance;
    private SaveLoadManager saveManager;

    private bool isPaused = false;
    private bool wasAlreadyPaused = false;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            saveManager = SaveLoadManager.Instance;

            if (saveManager == null)
            {
                Debug.LogError("SaveGameManager not found in scene.");
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                StartCoroutine(PauseGame());
            }
        }
    }

    // Pauses the game and shows the pause menu
    public IEnumerator PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        AudioManager.instance.PlayOneShot(FMODEvents.instance.WindowOpen);
        yield return new WaitForSecondsRealtime(0.4f);
        AudioManager.instance.SetPaused(true); // Pause audio
    }

    // Resumes the game and hides the pause menu
    public void ResumeGame()
    {
        AudioManager.instance.SetPaused(false); // Resume audio
        pauseMenuUI.SetActive(false);
        audioMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        AudioManager.instance.PlayOneShot(FMODEvents.instance.WindowClose);

    }

    public void openOptionsMenu()
    {
        optionsMenuUI.SetActive(true);
        pauseMenuUI.SetActive(false);
    }

    public void openAudioMenu()
    {
        audioMenuUI.SetActive(true);
        pauseMenuUI.SetActive(false);
    }

    public void returnToPauseMenu()
    {
        optionsMenuUI.SetActive(false);
        audioMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void OnSaveClicked()
    {
        saveManager?.Save();
    }

    public void OnLoadClicked()
    {
        saveManager?.Load();
    }

    // Quit the game
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}