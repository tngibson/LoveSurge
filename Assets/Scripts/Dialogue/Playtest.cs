using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class Playtest : MonoBehaviour
{
    [SerializeField] public int power1;
    [SerializeField] public int power2;
    [SerializeField] public int power3;
    [SerializeField] public string type1;
    [SerializeField] public string type2;
    [SerializeField] public string type3;
    List<ConvoTopic> convoTopics;
    [SerializeField] public ConvoTopic convoTopic1;
    [SerializeField] public ConvoTopic convoTopic2;
    [SerializeField] public ConvoTopic convoTopic3;

    [SerializeField] TextMeshProUGUI dateTextOutput;
    [SerializeField] TextMeshProUGUI playerTextOutput;
    [SerializeField] FileInfo source;
    protected FileInfo playerSource;
    public StreamReader reader = null;
    public StreamReader playerReader = null;
    protected string text = " ";
    protected string playerText = " ";
    float timer = 3f;
    // Start is called before the first frame update
    protected void Start()
    {
        source = new FileInfo("Assets/Assets/CelciText.txt");
        reader = source.OpenText();
        playerSource = new FileInfo("Assets/Assets/playerText.txt");
        playerReader = playerSource.OpenText();
        showTopics();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected void setTopics(int num1,string topic1, int num2, string topic2, int num3, string topic3)
    {
        convoTopic1.setNum(num1);
        convoTopic1.setTopic(topic1);
        //convoTopics.Add(convoTopic1);
        convoTopic2.setNum(num2);
        convoTopic2.setTopic(topic2);
        //convoTopics.Add(convoTopic2);
        convoTopic3.setNum(num3);
        convoTopic3.setTopic(topic3);
    }

    void showTopics()
    {
        setTopics(power1, type1, power2, type2, power3, type3);
    }
    public void readText(TextMeshProUGUI convoTextOuput, StreamReader stream)
    {
        if (text != null)
        {
            text = stream.ReadLine();
            convoTextOuput.text = text;
        }
    }
    void Timer()
    {
        if (timer > 0f) { timer -= Time.deltaTime; }
        else { timer = 0f; }

    }

}

