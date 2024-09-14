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
    [SerializeField] private Image background;

    // Icon-related fields
    [SerializeField] private GameObject attrIconContainer;
    private SpriteRenderer iconRenderer;

    // Icon sprites for different attributes
    [SerializeField] private Sprite chaIcon;
    [SerializeField] private Sprite cleIcon;
    [SerializeField] private Sprite couIcon;
    [SerializeField] private Sprite creIcon;

    // Drop zone and game manager references
    [SerializeField] private GameObject dropZone;
    [SerializeField] private Dropzone dropZoneScript;
    [SerializeField] private GameManager gameManager;

    // To track if this convo topic has been clicked
    public bool isClicked = false;

    // Called when the script instance is being loaded
    void Awake()
    {
        // Cache references to the SpriteRenderer and GameManager
        iconRenderer = attrIconContainer.GetComponentInChildren<SpriteRenderer>();
        gameManager = FindAnyObjectByType<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize drop zone and set the appropriate icon based on the convoAttribute
        dropZone = GameObject.Find("Dropzone");
        dropZoneScript = dropZone.GetComponent<Dropzone>();
        SetIcon();
    }

    // Set the conversation topic and update the UI
    public void SetTopic(string topic)
    {
        convoAttribute = topic;
        attributeText.text = topic;
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
                iconRenderer.sprite = chaIcon;
                iconRenderer.color = Color.black;
                break;
            case "cre":
                iconRenderer.sprite = creIcon;
                break;
            case "cou":
                iconRenderer.sprite = couIcon;
                break;
            case "cle":
                iconRenderer.sprite = cleIcon;
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
        if (!isClicked)
        {
            // Highlight the background color when clicked
            background.color = new Color(1, 0.92f, 0.016f, 1);
            isClicked = true;

            // Set this as the selected convo topic in the drop zone and game manager
            dropZoneScript.selectedConvoTopic = this;
            gameManager.currentConvoTopic = this;
        }
    }

    // Return whether this topic has been clicked
    public bool GetIsClicked()
    {
        return isClicked;
    }
}

