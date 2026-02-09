using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
//using FMOD.Studio;


public class Playtest : MonoBehaviour
{
    // Text is typing
    public bool isWriting = false;

    // Audio
    // private EventInstance playerVoice;
    // private EventInstance dateVoice;

    // Serialized fields for conversation topics, power levels, and types
    [SerializeField] public string type1;
    [SerializeField] public string type2;
    [SerializeField] public string type3;
    [SerializeField] public string type4;
    [SerializeField] public string label1;
    [SerializeField] public string label2;
    [SerializeField] public string label3;
    [SerializeField] public string label4;
    [SerializeField] public bool addExtraType1Line = false;
    [SerializeField] public bool addExtraType2Line = false;
    [SerializeField] public bool addExtraType3Line = false;
    [SerializeField] public bool addExtraType4Line = false;

    // List to hold conversation topics
    private List<ConvoTopic> convoTopics;

    // List to hold colorable UI sprites
    [SerializeField] private List<Image> uiOpaqueImages;
    [SerializeField] private Color uiOpaqueColor;
    [SerializeField] private List<Image> uiImages;
    [SerializeField] private Color uiColor;

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

    // Field for the date character so we can change their sprites
    [SerializeField] public Image dateCharacter;

    private Vector3 originalPosition;
    private Coroutine jumpCoroutine;

    //[SerializeField] private AudioSource dateTextSFX;
    //[SerializeField] private AudioSource playerTextSFX;
    // Start is called before the first frame update
    protected void Start()
    {
        //InitializeFileSources();
        ShowTopics();
        SetUIColor();
        originalPosition = dateCharacter.transform.localPosition;
    }

    // Sets the conversation topics with power and topic names
    protected void SetTopics(string topic1, string label1, string topic2, string label2, string topic3, string label3, string topic4, string label4)
    {
        convoTopic1.SetTopic(topic1, label1);

        convoTopic2.SetTopic(topic2, label2);

        convoTopic3.SetTopic(topic3, label3);

        convoTopic4.SetTopic(topic4, label4);
    }

    // Displays the conversation topics
    public void ShowTopics()
    {
        SetTopics(type1, label1, type2, label2, type3, label3, type4, label4);
    }

    public void SetUIColor()
    {
        foreach (var uiSprite in uiOpaqueImages)
        {
            uiSprite.color = uiOpaqueColor;
        }

        foreach (var uiSprite in uiImages)
        {
            uiSprite.color = uiColor;
        }
    }

    // Plays player text sounds
    public void ReadPlayerText()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.PlayerVoice, this.transform.position);
    }

    // Changes the character pose and plays sounds
    public void ReadDateText(Sprite characterPose)
    {
        if (characterPose != null)
        {
            // Change the sprite of the date character
            dateCharacter.sprite = characterPose;
        }

        // Play date voice sound
        AudioManager.instance.PlayOneShot(FMODEvents.instance.DateVoice, transform.position);

        // Start the jump animation coroutine
        if (jumpCoroutine != null)
        {
            StopCoroutine(jumpCoroutine);
            dateCharacter.transform.localPosition = originalPosition;
            jumpCoroutine = StartCoroutine(JumpAnimation());
        }
        else
        {
            jumpCoroutine = StartCoroutine(JumpAnimation());
        }
    }

    // Coroutine to handle the jump animation
    private IEnumerator JumpAnimation()
    {
        // Define the jump height and speed
        float jumpHeight = 50f;
        float jumpSpeed = 0.15f;

        Debug.Log("Original Position: " + originalPosition);
        
        // Move the character up
        Vector3 targetPosition = originalPosition + new Vector3(0, jumpHeight, 0);
        Debug.Log("Target Position: " + targetPosition);
        float elapsedTime = 0f;

        // Animate the upward movement
        while (elapsedTime < jumpSpeed)
        {
            float t = Mathf.Clamp01(elapsedTime / jumpSpeed);
            dateCharacter.transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the position is set exactly to the target
        dateCharacter.transform.localPosition = targetPosition;

        // Reset time for downward movement
        elapsedTime = 0f;

        // Animate the downward movement
        while (elapsedTime < jumpSpeed)
        {
            float t = Mathf.Clamp01(elapsedTime / jumpSpeed);
            dateCharacter.transform.localPosition = Vector3.Lerp(targetPosition, originalPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the position is set exactly to the original
        Debug.Log("Setting to Original Position: " + originalPosition);
        dateCharacter.transform.localPosition = originalPosition;
        Debug.Log("Returned to Original Position: " + dateCharacter.transform.localPosition);
    }
}
