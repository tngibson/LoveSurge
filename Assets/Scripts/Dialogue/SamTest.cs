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

        // Show the topics at the start
        ShowTopics();
        playerNameText.text = "Player"; // For now, since we are not taking player's name, this is just set to Player. Later, it will be set a different way
        dateNameText.text = "Sam"; // Sets the Date's Name to Lotte
    }

    // Sets the conversation topics with provided power and topic names
    protected new void SetTopics(int num1, string topic1, int num2, string topic2, int num3, string topic3, int num4, string topic4)
    {
        convoTopic1.SetNum(num1);
        convoTopic1.SetTopic(topic1);

        convoTopic2.SetNum(num2);
        convoTopic2.SetTopic(topic2);

        convoTopic3.SetNum(num3);
        convoTopic3.SetTopic(topic3);

        convoTopic4.SetNum(num4);
        convoTopic4.SetTopic(topic4);
    }

    // Displays the conversation topics
    private void ShowTopics()
    {
        SetTopics(power1, type1, power2, type2, power3, type3, power4, type4);
    }
}
