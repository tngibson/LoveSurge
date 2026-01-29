using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using static LocationManager;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton instance for global access

    // UI elements for displaying game information
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI deckCountText;
    [SerializeField] private GameObject endGameText;
    [SerializeField] private GameObject fullHandText;

    // Stress values
    [SerializeField] private float currentStressAmt = 0f;
    [SerializeField] private float maxStressAmt = 10f;

    // Card prefabs
    [SerializeField] private GameObject chaCard;
    [SerializeField] private GameObject couCard;
    [SerializeField] private GameObject cleCard;
    [SerializeField] private GameObject creCard;

    // Gameplay elements
    [SerializeField] private GameObject itemCanvasPrefab;
    [SerializeField] private PlayerArea playerArea;
    [SerializeField] public ConvoTopic currentConvoTopic;
    [SerializeField] private TopicContainer topicContainer;
    [SerializeField] private PlayerDeckScript deckContainer;
    [SerializeField] private Dropzone dropzone;
    [SerializeField] private DiscardPile discard;
    [SerializeField] private GameObject discardBin;
    [SerializeField] private ReserveManager reserveManager;

    public List<string> categories = new List<string> { "Cha", "Cou", "Cle", "Cre" };

    // Buttons
    [SerializeField] private GameObject endTurnButton;
    [SerializeField] private GameObject mapButton;

    [SerializeField] private int CurrentCharacterIndex; // which character date this belongs to | 0 - Noki, 1 - Celci, 2 - Lotte
    private int currentScore = -1;
    private bool isTopicSelected;

    public bool IsTopicSelected { get; set; }

    [SerializeField] private int handSize = 4;
    private bool isHandPlayable = false;
    private GameObject itemCanvasInstance;

    [SerializeField] private MapScript mapButtonScript;

    [SerializeField] private int comboSurge = 0;
    public int ComboSurge
    {
        get => comboSurge;
        set { comboSurge = math.clamp(value, 0, 4); }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        StressBar.instance?.UpdateStressBar();
        SetConvoStart();
    }

    private void Start()
    {
        if(itemCanvasInstance == null)
        {
            itemCanvasInstance = Instantiate(itemCanvasPrefab, transform.parent);
            itemCanvasInstance.name = "ItemCanvas";
            itemCanvasInstance.transform.SetSiblingIndex(itemCanvasInstance.transform.parent.childCount - 2);
            RefreshUsableItem();
        }
        fullHandText.SetActive(false);
        scoreText.text = "Score: 0";
        MusicManager.SetParameterByName("dateProgress", 0);
        //Debug.Log("Start Music");
    }

    public void SetConvoStart()
    {
        if (isTopicSelected)
            isTopicSelected = false;
    }

    public void OnEndTurn()
    {
        if (playerArea.CardsInHand.Count < handSize && deckContainer.Deck.Count > 0)
        {
            int missingCards = handSize - playerArea.CardsInHand.Count;
            fullHandText.SetActive(false);

            for (int i = 0; i < missingCards; i++)
            {
                Card card = deckContainer.Draw();
                if (card != null)
                {
                    deckContainer.RemoveCard(card);
                    card.transform.SetParent(playerArea.transform);
                    playerArea.AddCards(card);
                }
            }

            deckCountText.text = deckContainer.Deck.Count.ToString();
        }
        else if (deckContainer.Deck.Count <= 0)
        {
            fullHandText.SetActive(true);
            fullHandText.GetComponentInChildren<TextMeshProUGUI>().text = "Your Deck is Empty!";
        }
        else
        {
            fullHandText.SetActive(true);
            fullHandText.GetComponentInChildren<TextMeshProUGUI>().text = "Your Hand is Full!";
        }

        if (currentConvoTopic != null)
            dropzone.ScoreCards();

        dropzone.ResetForNewTurn();

        if (topicContainer.convoTopics.Count <= 0)
        {
            EndGameFullWin();
        }
        else if (deckContainer.Deck.Count <= 0 && (playerArea.CardsInHand.Count == 0 || checkHandPlayable() == false))
        {
            EndGameLoss();
        }

        dropzone.ResetBoosts();
        comboSurge = 0;
    }

    // Retrieve the active character�s date data dynamically
    private DateData GetActiveCharacter()
    {
        return LocationManager.Instance.characterDates[CurrentCharacterIndex];
    }

    public void EndGameHalfWin()
    {
        var data = GetActiveCharacter();

        switch (data.currentDate)
        {
            case DateNum.Date1:
                data.date1Stage = Date1Stage.CardGame;
                mapButtonScript.locName = $"{data.name}Date1SkillCheck1";
                break;
            case DateNum.Date2:
                data.date2Stage = Date2Stage.CardGame;
                mapButtonScript.locName = $"{data.name}Date2SkillCheck1";
                break;
            case DateNum.Date3:
                data.date3Stage = Date3Stage.CardGame;
                mapButtonScript.locName = $"{data.name}Date3SkillCheck1";
                break;
        }

        ShowMapButton($"{data.name} wants to talk more closely with you...", 1);
        Player.instance.ReturnItem();
    }

    public void EndGameFullWin()
    {
        var data = GetActiveCharacter();

        switch (data.currentDate)
        {
            case DateNum.Date1:
                data.date1Stage = Date1Stage.Done;
                mapButtonScript.locName = $"{data.name}Date1DeepConvo1";
                break;
            case DateNum.Date2:
                data.date2Stage = Date2Stage.Done;
                mapButtonScript.locName = $"{data.name}Date2SkillCheck2";
                break;
            case DateNum.Date3:
                data.date3Stage = Date3Stage.Done;
                mapButtonScript.locName = $"{data.name}Date3SkillCheck3";
                break;
        }

        ShowMapButton("You Win, Congratulations!", 1);

        // Update the location so LocationManager can advance to the next date
        LocationManager.Instance.TryBindMapScript(mapButtonScript);
        Player.instance.ReturnItem();
    }

    public void EndGameLoss()
    {
        var data = GetActiveCharacter();

        ShowMapButton("It's getting late, you should be heading back...", 3);
        Debug.Log("Bad End");

        if (!data.dateStarted)
        {
            data.dateStarted = true;
        }

        data.isPlayable = false;
        LocationManager.Instance.TryBindMapScript(mapButtonScript);
        Player.instance.ReturnItem();
    }

    public void ShowMapButton(string message, int musicProgress)
    {
        endGameText.GetComponentInChildren<TextMeshProUGUI>().text = message;
        endGameText.SetActive(true);
        endTurnButton.SetActive(false);
        discardBin.SetActive(false);
        mapButton.SetActive(true);
        MusicManager.SetParameterByName("dateProgress", musicProgress);
    }

    public void UpdateScore(int score)
    {
        currentScore += score;
        scoreText.text = $"Score: {currentScore}";
    }

    public void ResetConvoTopic()
    {
        currentConvoTopic = null;
        isTopicSelected = false;
        topicContainer.EnableButtons();
    }

    public void UpdateEndTurnButton(bool state)
    {
        if (endTurnButton.GetComponent<Button>().interactable != state)
        {
            endTurnButton.GetComponent<Button>().interactable = state;
        }
    }

    private bool checkHandPlayable()
    {
        isHandPlayable = false;
        foreach (Card card in playerArea.CardsInHand)
        {
            if (dropzone.CanPlaceCard(card))
                isHandPlayable = true;
        }

        if (dropzone.CanPlaceCard(reserveManager.GetCurrentPlayableCard()))
            isHandPlayable = true;

        return isHandPlayable;
    }

    public void RefreshUsableItem()
    {
        if(itemCanvasInstance == null)
        {
            Debug.LogError("Item Canvas is null and may not be in the scene!");
            return;
        }

        itemCanvasInstance.TryGetComponent(out Socket socket);
        for(int i = 0; i < Player.instance.collectedItems.Count; i++)
        {
            GameItem item = Player.instance.collectedItems[i];
            socket.AddToSocket(item.gameObject);
        }
    }
}
