using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject nameInputContainer;
    public Button newGameButton;
    public Button resumeGameButton;

    [Header("Confirmation Modal")]
    public GameObject confirmDeleteModal;

    [Header("Scene")]
    public string firstSceneName;

    private bool hasSave;

    void Start()
    {
        SetupUI();
    }

    void SetupUI()
    {
        hasSave = SaveLoadManager.Instance.HasSaveFile();

        confirmDeleteModal.SetActive(false);

        if (hasSave)
        {
            nameInputContainer.SetActive(false);
            resumeGameButton.gameObject.SetActive(true);
        }
        else
        {
            nameInputContainer.SetActive(true);
            resumeGameButton.gameObject.SetActive(false);
        }
    }

    // ================= BUTTON METHODS =================

    public void OnNewGamePressed()
    {
        if (hasSave)
        {
            confirmDeleteModal.SetActive(true);
        }
        else
        {
            StartNewGame();
        }
    }

    public void OnConfirmDeletePressed()
    {
        SaveLoadManager.Instance.DeleteSave();
        confirmDeleteModal.SetActive(false);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnCancelDeletePressed()
    {
        confirmDeleteModal.SetActive(false);
    }

    public void OnResumePressed()
    {
        SaveLoadManager.Instance.LoadGame();
    }

    void StartNewGame()
    {
        SceneManager.LoadScene(firstSceneName);
    }
}