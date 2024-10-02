using System.IO;
using UnityEngine;

public class SamTest : Playtest
{
    // Start is called before the first frame update
    protected new void Start()
    {
        // Set the file paths for player and Lotte text
        /*
        base.playerSource = new FileInfo("Assets/Assets/DialogueResources/Dialogue Files/LotteText.txt");
        reader = base.playerSource.OpenText();

        playerSource = new FileInfo("Assets/Assets/DialogueResources/Dialogue Files/playerText.txt");
        playerReader = playerSource.OpenText();
        */
        ShowTopics();
        playerNameText.text = "Player"; // For now, since we are not taking player's name, this is just set to Player. Later, it will be set a different way
        dateNameText.text = "Sam"; // Sets the Date's Name to Lotte
    }
}
