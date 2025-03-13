using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NameInputField : MonoBehaviour
{
    public TMP_InputField inputField; // Reference to your InputField component
    public Player player;

    private void Start()
    {
        // Retrieve the stored user input from PlayerPrefs when the scene starts
        string storedInput = PlayerPrefs.GetString("PlayerName", "");

        if (player != null)
        {
            player.SetName(storedInput);
        }

        // Select and focus on the input field at the start
        StartCoroutine(SetInputFieldSelected());
    }

    private IEnumerator SetInputFieldSelected()
    {
        yield return null; // Wait a frame to ensure UI is initialized
        inputField.Select();
        inputField.ActivateInputField();
    }

    // This method can be called from a button click or any other event to save the input text
    public void SaveInputText()
    {
        string playerName = inputField.text; // Get the input text from the input field
        if (playerName != "")
        {
            PlayerPrefs.SetString("PlayerName", playerName); // Save the input text to PlayerPrefs
            player.SetName(playerName);
        }
        else
        {
            PlayerPrefs.SetString("PlayerName", "Player");
            player.SetName(playerName);
        }
        PlayerPrefs.Save(); // Ensure the changes are saved immediately
    }

    private void OnApplicationQuit()
    {
        // Clear PlayerPrefs data when the game is ended (THIS IS A TEMP METHOD FOR DEMO PURPOSES, PLEASE REMOVE WHEN WE START USING GAME STATES)
        PlayerPrefs.DeleteAll();
    }
}
