using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReserveSlot : MonoBehaviour
{
    [SerializeField] private Card[] cardPrefabs; // Array of card prefabs to choose from

    [SerializeField] private GameObject reserveSlot;

    public Image cardImage; // Reference to the card image for UI changes
    public DragDrop card;   // The card currently in this slot
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
            card.SetDraggable(true);
            cardImage.color = GetCardColor(); // Set to the card's specific color
        }
        else
        {
            card.SetDraggable(false);
            cardImage.color = new Color(0.3f, 0.3f, 0.3f, 1f); // Gray out effect
        }
    }

    public void GenerateNewCard()
    {
        // Add the starting card to the dropzone slot
        int randomAttribute = Random.Range(0, cardPrefabs.Length);
        int randomPower = Random.Range(1, 6);

        Card startingCard = Instantiate(cardPrefabs[randomAttribute]);
        card = startingCard.GetComponent<DragDrop>();
        cardImage = startingCard.background;
        startingCard.Power = randomPower;

        startingCard.transform.SetParent(reserveSlot.transform, false); // Parent it to the ReserveSlot
        startingCard.isReserveCard = true;
    }

    private Color GetCardColor()
    {
        if (card.GetComponent<ChaCard>() != null)
        {
            return cardColors["ChaCard"];
        }
        else if (card.GetComponent<CreCard>() != null)
        {
            return cardColors["CreCard"];
        }
        else if (card.GetComponent<CleCard>() != null)
        {
            return cardColors["CleCard"];
        }
        else if (card.GetComponent<CouCard>() != null)
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
