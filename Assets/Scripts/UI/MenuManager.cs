using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Instructions panel
    [SerializeField] private GameObject instructions;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Ensure the instructions panel is hidden on startup
        instructions.SetActive(false);
    }

    // Loads the CelciTest scene when their button is clicked
    public void OnCelciClick()
    {
        SceneManager.LoadScene("CelciTest");
    }

    // Loads the LotteTest scene when their button is clicked
    public void OnLotteClick()
    {
        SceneManager.LoadScene("LotteTest");
    }

    // Displays the instructions panel
    public void OnInstructionClick()
    {
        instructions.SetActive(true);
    }

    // Hides the instructions panel
    public void OnBackClick()
    {
        instructions.SetActive(false);
    }
}
