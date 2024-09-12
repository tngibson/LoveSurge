using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopicContainer : MonoBehaviour
{
    [SerializeField] public List<ConvoTopic> convoTopics;
    [SerializeField] public List<ConvoTopic> doneConvos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        disableButtons();
    }
    public void disableButtons()
    {
        
        for (int i = 0; i < convoTopics.Count; i++) 
        {
            if (convoTopics[i] != null)
            {
                if (convoTopics[i].getIsClicked())
                {
                    for (int j = 0; j < convoTopics.Count; j++)
                    {
                        if (convoTopics[j].getIsClicked() == false)
                        {
                            convoTopics[j].gameObject.GetComponentInChildren<Button>().enabled = false;
                        }
                    }
                    return;
                }
            }
        }
    }
    public void enableButtons()
    {
        for (int i = 0; i < convoTopics.Count; i++)
        {
            if (convoTopics[i] == null){continue;}
            convoTopics[i].gameObject.GetComponentInChildren<Button>().enabled = true;
        }
    }
  
}

