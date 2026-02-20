using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject audioMenuUI;
    [SerializeField] private GameObject optionsMenuUI;
    public static PauseMenu instance;

    private bool isPaused = false;
    private bool wasAlreadyPaused = false;

    [SerializeField] private string titleSceneName = "TitleScreen";

    void Start()
    {
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
            isPaused = false;
            AudioManager.instance?.SetPaused(false);
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Update()
    {
        if (!CanPause())
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                StartCoroutine(PauseGame());
        }
    }

    bool CanPause()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        return currentScene != titleSceneName;
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

    public void ForceResumeAfterLoad()
    {
        pauseMenuUI.SetActive(false);
        audioMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);

        isPaused = false;
        AudioManager.instance?.SetPaused(false);
    }

    // Quit the game
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}