using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class LotteTest : Playtest
{
    // Start is called before the first frame update
    protected new void Start()
    {
        base.playerSource = new FileInfo("Assets/Assets/LotteText.txt");
        reader = base.playerSource.OpenText();
        playerSource = new FileInfo("Assets/Assets/playerText.txt");
        playerReader = playerSource.OpenText();
        showTopics();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected new void setTopics(int num1,string topic1, int num2, string topic2, int num3, string topic3)
    {
        convoTopic1.SetNum(num1);
        convoTopic1.SetTopic(topic1);
        //convoTopics.Add(convoTopic1);
        convoTopic2.SetNum(num2);
        convoTopic2.SetTopic(topic2);
        //convoTopics.Add(convoTopic2);
        convoTopic3.SetNum(num3);
        convoTopic3.SetTopic(topic3);
    }

    void showTopics()
    {
        setTopics(power1, type1, power2, type2, power3, type3);
    }

}

