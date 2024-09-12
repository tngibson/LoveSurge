using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConvoTopic : MonoBehaviour
{
    [SerializeField] private string convoAttribute;
    public string ConvoAttribute { get { return convoAttribute; } set { convoAttribute = value; } }
    [SerializeField] private int powerNum;
    public int PowerNum { get { return powerNum; } set { powerNum = value; } }
    [SerializeField] TextMeshProUGUI attributeText;
    [SerializeField] public TextMeshProUGUI numText;
    [SerializeField] Image Background;
    [SerializeField] GameObject AttrIconContainer;
    SpriteRenderer iconRenderer;
    Sprite AttrIcon;

    [SerializeField] GameObject dropZone;
    [SerializeField] Dropzone DropZone;
    [SerializeField] GameManager gameManager;

    [SerializeField] Sprite chaIcon;
    [SerializeField] Sprite cleIcon;
    [SerializeField] Sprite couIcon;
    [SerializeField] Sprite creIcon;

    public bool isClicked = false;
    void Awake()
    {
        iconRenderer = AttrIconContainer.GetComponentInChildren<SpriteRenderer>();
        gameManager = FindAnyObjectByType<GameManager>();
    }
    void Start()
    {
        dropZone = GameObject.Find("Dropzone");
        DropZone = dropZone.GetComponent<Dropzone>();
        setIcon();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void setTopic(string topic)
    {
        convoAttribute = topic;
        attributeText.text = topic;
        setIcon();
    }
    public void setNum(int numInput)
    {
        powerNum = numInput;
        numText.text = numInput.ToString();
    }
    public void setIcon()
    {
        if (convoAttribute.ToLower() == "cha")
        {
            iconRenderer.sprite = chaIcon;
            iconRenderer.GetComponent<SpriteRenderer>().color = Color.black;
        }
        else if (convoAttribute.ToLower() == "cre")
        {
            iconRenderer.sprite = creIcon;
        }
        else if (convoAttribute.ToLower() == "cou")
        {
            iconRenderer.sprite = couIcon;
        }
        else if (convoAttribute.ToLower() == "cle")
        {
            iconRenderer.sprite = cleIcon;
        }
    }

    public int getPowerNum()
    {
        return powerNum;
    }
    public string getTopic()
    {
        return convoAttribute;
    }
    public void onButtonPress()
    {
        if (!isClicked) { Background.color = new Color(1, 0.92f, 0.016f, 1); isClicked = true; DropZone.selectedConvoTopic = this; gameManager.currentConvoTopic = this; }
    }
    public bool getIsClicked() { return isClicked; }
}
