using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConvoTopic : MonoBehaviour
{
    // Serialized fields for attribute and power number
    [SerializeField] private string convoAttribute;
    public string ConvoAttribute { get { return convoAttribute; } set { convoAttribute = value; } }

    [SerializeField] private int powerNum;
    public int PowerNum { get { return powerNum; } set { powerNum = value; } }

    // UI components for displaying attribute and power number
    [SerializeField] private TextMeshProUGUI attributeText;
    [SerializeField] public TextMeshProUGUI numText;
    [SerializeField] public TextMeshProUGUI topicLabelText;
    [SerializeField] public TextMeshProUGUI convoText;
    [SerializeField] public GameObject finishedText;
    [SerializeField] public GameObject bustedText;
    [SerializeField] public Image background;

    // Icon-related fields
    [SerializeField] private SpriteRenderer iconRenderer;

    // Icon sprites for different attributes
    [SerializeField] private Sprite chaIcon;
    [SerializeField] private Sprite cleIcon;
    [SerializeField] private Sprite couIcon;
    [SerializeField] private Sprite creIcon;
    // Colors for different attributes CHANGE THEM ASAP
    [SerializeField] private Color chaColor;
    [SerializeField] private Color cleColor;
    [SerializeField] private Color couColor;
    [SerializeField] private Color creColor;
    // Drop zone and game manager references
    [SerializeField] private GameObject dropZone;
    [SerializeField] private Dropzone dropZoneScript;
    [SerializeField] private GameManager gameManager;

    // To track if this convo topic has been clicked
    public bool isClicked = false;

    // Tracks if the convo topic is failed
    public bool isFailed = false;

    // To track if the player is locked into a convo topic
    public bool isLocked = false;

    public RectTransform buttonTransform;
    public float pressAmount = 5f; // The amount the button moves down when clicked

    // Called when the script instance is being loaded
    void Awake()
    {
        // Cache references to the SpriteRenderer and GameManager
        gameManager = FindAnyObjectByType<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize drop zone and set the appropriate icon based on the convoAttribute
        dropZone = GameObject.Find("ConvoTopicPanel");
        SetIcon();
        changeBGColor();
    }

    // Set the conversation topic and update the UI
    public void SetTopic(string topic, string topicLabel)
    {
        convoAttribute = topic;
        attributeText.text = topic;
        topicLabelText.text = topicLabel;
        SetIcon(); // Update the icon based on the new topic
    }

    // Set the power number and update the UI
    public void SetNum(int numInput)
    {
        powerNum = numInput;
        numText.text = numInput.ToString();
    }

    // Set the appropriate icon based on the conversation attribute
    public void SetIcon()
    {
        switch (convoAttribute.ToLower())
        {
            case "cha":
            case "charisma":
                iconRenderer.sprite = chaIcon;
                break;
            case "cre":
            case "creativity":
                iconRenderer.sprite = creIcon;
                break;
            case "cou":
            case "courage":
                iconRenderer.sprite = couIcon;
                break;
            case "cle":
            case "cleverness":
                iconRenderer.sprite = cleIcon;
                break;
            default:
                Debug.LogWarning("Unknown convoAttribute: " + convoAttribute);
                break;
        }
    }


    //changes topic color
    void changeBGColor()
    {
        switch (convoAttribute.ToLower())
        {
            case "cha":
            case "charisma":
                background.color = chaColor;
                break;
            case "cre":
            case "creativity":
                background.color = creColor;
                break;
            case "cou":
            case "courage":
                background.color = couColor;
                break;
            case "cle":
            case "cleverness":
                background.color = cleColor;
                break;
            default:
                Debug.LogWarning("Unknown convoAttribute: " + convoAttribute);
                break;
        }
    }
    // Return the power number
    public int GetPowerNum()
    {
        return powerNum;
    }

    // Return the conversation topic
    public string GetTopic()
    {
        return convoAttribute;
    }

    // Called when the button is pressed, changes background color and updates state
    public void OnButtonPress()
    {
        if (!isClicked && !isFailed && powerNum > 0)
        {
            ToggleClick(isClicked);

            // Highlight the background color when clicked
            background.color = new Color(1, 0.95f, 0.5f, 1); // Pastel yellow
            isClicked = true;
            convoText.text = topicLabelText.text;

            // Set this as the selected convo topic in the drop zone and game manager
            dropZoneScript.selectedConvoTopic = this;
            gameManager.IsTopicSelected = true;
            gameManager.currentConvoTopic = this;
        }
        else if (isClicked && !isFailed && powerNum > 0 && !isLocked)
        {
            ToggleClick(isClicked);

            gameManager.currentConvoTopic = this;
            changeBGColor();
            isClicked = false;
            convoText.text = "Awaiting Topic...";

            //Deselects current convo topic when clicked
            dropZoneScript.selectedConvoTopic = null;
            gameManager.IsTopicSelected = false;
            gameManager.ResetConvoTopic();
            dropZoneScript.ReturnCards();
        }
    }

    // Return whether this topic has been clicked
    public bool GetIsClicked()
    {
        return isClicked;
    }

    public void ToggleClick(bool clicked)
    {
        if (clicked)
        {
            // Move the button down when clicked
            buttonTransform.anchoredPosition = new Vector2(buttonTransform.anchoredPosition.x, buttonTransform.anchoredPosition.y + pressAmount);
        }
        else
        {
            // Move the button down when clicked
            buttonTransform.anchoredPosition = new Vector2(buttonTransform.anchoredPosition.x, buttonTransform.anchoredPosition.y - pressAmount);
        }
    }
}

