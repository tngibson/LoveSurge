using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopicContainer : MonoBehaviour
{
    // List of conversation topics and completed topics
    [SerializeField] public List<ConvoTopic> convoTopics;
    [SerializeField] public List<ConvoTopic> doneConvos;
    [SerializeField] public List<ConvoTopic> failedConvos;

    // Update is called once per frame
    void Update()
    {
        DisableButtons();
    }

    // Disables all buttons except the clicked topic
    public void DisableButtons()
    {
        // Loop through each conversation topic to check if any are clicked
        foreach (var topic in convoTopics)
        {
            if (topic == null) continue;

            if (topic.GetIsClicked())
            {
                // Disable buttons for all non-clicked topics
                foreach (var otherTopic in convoTopics)
                {
                    if (otherTopic != null && !otherTopic.GetIsClicked())
                    {
                        otherTopic.gameObject.GetComponentInChildren<Button>().enabled = false;
                    }
                }
                return;  // Exit once a clicked topic is found and others are disabled
            }
        }
    }

    // Enables all buttons for the topics
    public void EnableButtons()
    {
        // Enable buttons for all conversation topics
        foreach (var topic in convoTopics)
        {
            if (topic != null)
            {
                topic.gameObject.GetComponentInChildren<Button>().enabled = true;
            }
        }
    }
}
