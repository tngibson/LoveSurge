using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Playtest : MonoBehaviour
{
    // Serialized fields for conversation topics, power levels, and types
    [SerializeField] public int power1;
    [SerializeField] public int power2;
    [SerializeField] public int power3;
    [SerializeField] public int power4;
    [SerializeField] public string type1;
    [SerializeField] public string type2;
    [SerializeField] public string type3;
    [SerializeField] public string type4;

    // List to hold conversation topics
    private List<ConvoTopic> convoTopics;

    // Serialized fields for the individual conversation topics
    [SerializeField] public ConvoTopic convoTopic1;
    [SerializeField] public ConvoTopic convoTopic2;
    [SerializeField] public ConvoTopic convoTopic3;
    [SerializeField] public ConvoTopic convoTopic4;

    // Serialized fields for the conversation dialogue (by topic)
    [SerializeField] public List<string> couLines;
    [SerializeField] public List<string> couSpeaker;
    [SerializeField] public List<Sprite> couSprites;

    [SerializeField] public List<string> chaLines;
    [SerializeField] public List<string> chaSpeaker;
    [SerializeField] public List<Sprite> chaSprites;

    [SerializeField] public List<string> cleLines;
    [SerializeField] public List<string> cleSpeaker;
    [SerializeField] public List<Sprite> cleSprites;

    [SerializeField] public List<string> creLines;
    [SerializeField] public List<string> creSpeaker;
    [SerializeField] public List<Sprite> creSprites;

    // Serialized fields for UI output
    [SerializeField] private TextMeshProUGUI dateTextOutput;
    [SerializeField] public TextMeshProUGUI dateNameText;
    [SerializeField] private TextMeshProUGUI playerTextOutput;
    [SerializeField] public TextMeshProUGUI playerNameText;

    // File handling variables
    /*
    [SerializeField] private FileInfo source;
    protected FileInfo playerSource;
    protected StreamReader reader = null;
    protected StreamReader playerReader = null;
    */

    // Getter methods for the readers
    /*
    public StreamReader GetReader() => reader;
    public StreamReader GetPlayerReader() => playerReader;
    */

    // Text for conversation output
    protected string text = " ";
    protected string playerText = " ";

    // Field for the date character so we can change their sprites
    [SerializeField] public Image dateCharacter;

    [SerializeField] private AudioSource textSFX;
    // Start is called before the first frame update
    protected void Start()
    {
        //InitializeFileSources();
        ShowTopics();
        playerNameText.text = "Player"; // For now, since we are not taking player's name, this is just set to Player. Later, it will be set a different way
        dateNameText.text = dateCharacter.gameObject.ToString();
    }

    // Initialize file sources for reading text
    /*
    private void InitializeFileSources()
    {
        source = new FileInfo("Assets/Assets/DialogueResources/Dialogue Files/CelciText.txt");
        reader = source.OpenText();
        playerSource = new FileInfo("Assets/Assets/DialogueResources/Dialogue Files/playerText.txt");
        playerReader = playerSource.OpenText();
    }
    */

    // Sets the conversation topics with power and topic names
    protected void SetTopics(int num1, string topic1, int num2, string topic2, int num3, string topic3, int num4, string topic4)
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

    // Reads text from a stream and updates the UI
    /*
    public void ReadText(TextMeshProUGUI convoTextOutput, StreamReader stream)
    {
        if (stream != null)
        {
            text = stream.ReadLine();
            textSFX.Play();
            convoTextOutput.text = text ?? ""; // If the text is null, set it to an empty string
        }
    }
    */

    // Changes the character pose and plays sounds
    public void ReadText(Sprite characterPose)
    {
        dateCharacter.sprite = characterPose;
        textSFX.Play();
    }
}
