using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReserveSlot : MonoBehaviour
{
    [SerializeField] private Card[] cardPrefabs; // Array of card prefabs to choose from

    [SerializeField] private GameObject reserveSlot;

    public Image cardImage; // Reference to the card image for UI changes
    public DragDrop cardDragDrop;   // The card currently in this slot
    public Card card;
    [SerializeField] private ReserveManager reserveManager;


    // Mapping card types to their respective playable colors
    private static readonly Dictionary<string, Color> cardColors = new Dictionary<string, Color>()
    {
        { "ChaCard", HexToColor("F4F1CC") },
        { "CreCard", HexToColor("C5E6D1") },
        { "CleCard", HexToColor("B7DCEA") },
        { "CouCard", HexToColor("EDC7E4") }
    };

    private void Awake()
    {
        GenerateNewCard();
    }

    public void SetPlayable(bool playable)
    {
        if (playable)
        {
            cardDragDrop.SetDraggable(true);
            cardImage.color = GetCardColor(); // Set to the card's specific color
        }
        else
        {
            cardDragDrop.SetDraggable(false);
            cardImage.color = new Color(0.3f, 0.3f, 0.3f, 1f); // Gray out effect
        }
    }

    public void GenerateNewCard()
    {
        // Add the starting card to the dropzone slot
        int randomAttribute = Random.Range(0, cardPrefabs.Length);
        int randomPower = Random.Range(1, 6);

        card = Instantiate(cardPrefabs[randomAttribute]);
        cardDragDrop = card.GetComponent<DragDrop>();
        cardImage = card.background;
        card.Power = randomPower;

        card.transform.SetParent(reserveSlot.transform, false); // Parent it to the ReserveSlot
        card.isReserveCard = true;
    }

    private Color GetCardColor()
    {
        if (cardDragDrop.GetComponent<ChaCard>() != null)
        {
            return cardColors["ChaCard"];
        }
        else if (cardDragDrop.GetComponent<CreCard>() != null)
        {
            return cardColors["CreCard"];
        }
        else if (cardDragDrop.GetComponent<CleCard>() != null)
        {
            return cardColors["CleCard"];
        }
        else if (cardDragDrop.GetComponent<CouCard>() != null)
        {
            return cardColors["CouCard"];
        }
        else
        {
            return Color.white;
        }
    }

    // Utility method to convert hex colors to Unity's Color type
    private static Color HexToColor(string hex)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString("#" + hex, out color))
        {
            return color;
        }
        return Color.white;
    }
}
