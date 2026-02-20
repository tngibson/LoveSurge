using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapEndGameController : MonoBehaviour
{
    [SerializeField] public GameObject endGameButton;
    public GameObject popupPanel;

    void Start()
    {
        popupPanel.SetActive(false);

        if (StoryProgressFlags.mainEvent29Completed &&
            !StoryProgressFlags.mainEvent3Started)
        {
            endGameButton.SetActive(true);
        }
        else
        {
            endGameButton.SetActive(false);
        }
    }

    public void OnEndGameButtonClicked()
    {
        endGameButton.SetActive(false);
        popupPanel.SetActive(true);
    }

    public void OnYesClicked()
    {
        StoryProgressFlags.mainEvent3Started = true;
        SceneManager.LoadScene("MainEvent3");
    }

    public void OnNoClicked()
    {
        popupPanel.SetActive(false);
        endGameButton.SetActive(true);
    }
}