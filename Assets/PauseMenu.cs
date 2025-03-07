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

    private bool isPaused = false;
    private bool wasAlreadyPaused = false;

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
                PauseGame();
            }
        }
    }

    // Pauses the game and shows the pause menu
    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        AudioManager.instance.SetPaused(true); // Pause audio
    }

    // Resumes the game and hides the pause menu
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        audioMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        AudioManager.instance.SetPaused(false); // Resume audio
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

    // Quit the game
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}