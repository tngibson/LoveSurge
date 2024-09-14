using System.IO;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class Playtest : MonoBehaviour
{
    // Serialized fields for conversation topics, power levels, and types
    [SerializeField] public int power1;
    [SerializeField] public int power2;
    [SerializeField] public int power3;
    [SerializeField] public string type1;
    [SerializeField] public string type2;
    [SerializeField] public string type3;

    // List to hold conversation topics
    private List<ConvoTopic> convoTopics;

    // Serialized fields for the individual conversation topics
    [SerializeField] public ConvoTopic convoTopic1;
    [SerializeField] public ConvoTopic convoTopic2;
    [SerializeField] public ConvoTopic convoTopic3;

    // Serialized fields for UI output
    [SerializeField] private TextMeshProUGUI dateTextOutput;
    [SerializeField] private TextMeshProUGUI playerTextOutput;

    // File handling variables
    [SerializeField] private FileInfo source;
    protected FileInfo playerSource;
    protected StreamReader reader = null;
    protected StreamReader playerReader = null;

    // Getter methods for the readers
    public StreamReader GetReader() => reader;
    public StreamReader GetPlayerReader() => playerReader;

    // Text for conversation output
    protected string text = " ";
    protected string playerText = " ";

    // Timer
    private float timer = 3f;

    // Start is called before the first frame update
    protected void Start()
    {
        InitializeFileSources();
        ShowTopics();
    }

    // Initialize file sources for reading text
    private void InitializeFileSources()
    {
        source = new FileInfo("Assets/Assets/CelciText.txt");
        reader = source.OpenText();
        playerSource = new FileInfo("Assets/Assets/playerText.txt");
        playerReader = playerSource.OpenText();
    }

    // Sets the conversation topics with power and topic names
    protected void SetTopics(int num1, string topic1, int num2, string topic2, int num3, string topic3)
    {
        convoTopic1.SetNum(num1);
        convoTopic1.SetTopic(topic1);

        convoTopic2.SetNum(num2);
        convoTopic2.SetTopic(topic2);

        convoTopic3.SetNum(num3);
        convoTopic3.SetTopic(topic3);
    }

    // Displays the conversation topics
    private void ShowTopics()
    {
        SetTopics(power1, type1, power2, type2, power3, type3);
    }

    // Reads text from a stream and updates the UI
    public void ReadText(TextMeshProUGUI convoTextOutput, StreamReader stream)
    {
        if (stream != null)
        {
            text = stream.ReadLine();
            convoTextOutput.text = text ?? ""; // If the text is null, set it to an empty string
        }
    }

    // Timer logic
    private void Timer()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            timer = 0f;
        }
    }
}
