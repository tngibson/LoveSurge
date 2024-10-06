using System.IO;
using UnityEngine;

public class SamTest : Playtest
{
    // Start is called before the first frame update
    protected new void Start()
    {
        ShowTopics();
        playerNameText.text = "Player"; // For now, since we are not taking player's name, this is just set to Player. Later, it will be set a different way
        dateNameText.text = "Noki"; // Sets the Date's Name to Lotte
    }
}
