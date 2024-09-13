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

}

