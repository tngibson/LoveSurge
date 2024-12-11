using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConvoTopic : MonoBehaviour
{
    // Serialized fields for attribute and power number
    [SerializeField] private string convoAttribute;
    public string ConvoAttribute { get { return convoAttribute; } set { convoAttribute = value; } }

    [SerializeField] private int currentTier = 1; // Current tier (1, 2, or 3)
    [SerializeField] public int tierPower; // Current tier's power
    [SerializeField] private int[] tierPowers = { 350, 400, 500 }; // Points for each tier

    public int CurrentTier => currentTier;
    public int TierPower => tierPower;

    // UI components for displaying attribute and power number
    [SerializeField] private TextMeshProUGUI attributeText;
    [SerializeField] public TextMeshProUGUI numText;
    [SerializeField] public TextMeshProUGUI topicLabelText;
    [SerializeField] public TextMeshProUGUI convoText;
    [SerializeField] public GameObject finishedText;
    [SerializeField] public GameObject bustedText;
    [SerializeField] public Image background;
    [SerializeField] public Color topicColor;

    // Icon-related fields
    [SerializeField] private Image icon;

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
    [SerializeField] private Dropzone dropZoneScript;
    [SerializeField] private GameManager gameManager;

    // To track if this convo topic has been clicked
    public bool isClicked = false;

    // To track if the player is locked into a convo topic
    public bool isLocked = false;

    // To track if this convo topic is completed
    public bool isCompleted = false;

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
        dropZoneScript = GameObject.Find("CardSlotsPanel").GetComponent<Dropzone>();
        SetIcon();
        changeBGColor();

        // Initialize the first tier
        currentTier = 1;
        tierPower = tierPowers[currentTier - 1];
        UpdatePowerUI();
    }

    // Set the conversation topic and update the UI
    public void SetTopic(string topic, string topicLabel)
    {
        convoAttribute = topic;
        attributeText.text = topic;
        topicLabelText.text = topicLabel;
        //SetIcon(); // Update the icon based on the new topic
    }

    // Set the power number and update the UI
    public void SetNum(int numInput)
    {
        tierPower = numInput;
        numText.text = numInput.ToString();
    }

    // Set the appropriate icon based on the conversation attribute
    public void SetIcon()
    {
        switch (convoAttribute.ToLower())
        {
            case "cha":
            case "charisma":
                icon.sprite = chaIcon;
                break;
            case "cre":
            case "creativity":
                icon.sprite = creIcon;
                break;
            case "cou":
            case "courage":
                icon.sprite = couIcon;
                break;
            case "cle":
            case "cleverness":
                icon.sprite = cleIcon;
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
                topicColor = chaColor;
                break;
            case "cre":
            case "creativity":
                background.color = creColor;
                topicColor = creColor;
                break;
            case "cou":
            case "courage":
                background.color = couColor;
                topicColor = couColor;
                break;
            case "cle":
            case "cleverness":
                background.color = cleColor;
                topicColor = cleColor;
                break;
            default:
                Debug.LogWarning("Unknown convoAttribute: " + convoAttribute);
                break;
        }
    }
    // Return the power number
    public int GetTierPower()
    {
        return tierPower;
    }

    // Return the conversation topic
    public string GetTopic()
    {
        return convoAttribute;
    }

    // Called when the button is pressed, changes background color and updates state
    public void OnButtonPress()
    {
        if (!isClicked && tierPower > 0)
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
        else if (isClicked && tierPower > 0 && !isLocked)
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

    public void ProgressToNextTier()
    {
        if (currentTier < tierPowers.Length)
        {
            currentTier++; // Move to the next tier
            tierPower = tierPowers[currentTier - 1]; // Update tier power
            isClicked = false;
            isLocked = false;
            UpdatePowerUI();

            Debug.Log($"Convo Topic progressed to Tier {currentTier}.");
        }
        else
        {
            // Mark the topic as completed
            isClicked = false;
            isLocked = false;
            isCompleted = true;
            Debug.Log("Convo Topic fully completed.");
        }
    }

    private void UpdatePowerUI()
    {
        numText.text = tierPower > 0 ? tierPower.ToString() : ""; // Show power if above 0
    }
}

