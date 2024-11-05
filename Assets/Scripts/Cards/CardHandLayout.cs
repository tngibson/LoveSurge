using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CardHandLayout : MonoBehaviour
{
    public float fanWidth = 500f;           // The width of the fan (how far cards spread)
    public float cardSpacing = 100f;        // Spacing between each card
    public float fanAngle = 30f;            // The angle spread of the fan
    public float hoverOffset = 50f;         // How much the card moves up when hovered
    public float hoverScale = 1.2f;         // The scale increase when hovered
    public float layoutUpdateDelay = 0.1f;  // Delay to batch updates if cards are added/removed in quick succession.
    public float maxYOffset = 50f;          // Maximum Y offset for cards on the outside of the fan.
    public float middleCardOffset = 5f; // Adjust as needed

    private List<Transform> cardTransforms = new List<Transform>(); // List of card transforms
    private bool isLayoutDirty = false;     // Flag to trigger layout recalculation

    // Store the original positions and rotations for each card to revert them on hover exit
    private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Quaternion> originalRotations = new Dictionary<Transform, Quaternion>();

    private void Start()
    {
        UpdateCardListAndLayout(); // Call to initially layout the cards
    }

    private void Update()
    {
        // Dynamically update the layout when children change
        if (transform.childCount != cardTransforms.Count)
        {
            UpdateCardListAndLayout();
        }
    }

    // Method to update the card list and trigger layout update
    public void UpdateCardListAndLayout()
    {
        cardTransforms.Clear(); // Clear the current card list
        originalPositions.Clear();
        originalRotations.Clear();

        foreach (Transform card in transform)
        {
            cardTransforms.Add(card); // Add all child cards to the list
            AddHoverListeners(card.gameObject); // Add hover listeners without removing existing ones
        }

        // Delay the layout update slightly to batch multiple changes
        if (!isLayoutDirty)
        {
            isLayoutDirty = true;
            Invoke(nameof(LayoutCards), layoutUpdateDelay);
        }
    }

    // Method to calculate and apply the fan layout for the cards
    private void LayoutCards()
    {
        isLayoutDirty = false;

        int cardCount = cardTransforms.Count;
        if (cardCount == 0) return;

        float middleIndex = (cardCount - 1) / 2f; // Middle index of the card array
        bool hasMiddleCard = cardCount % 2 != 0; // Check if there's an exact middle card

        for (int i = 0; i < cardCount; i++)
        {
            Transform card = cardTransforms[i];

            // Calculate the fan position and rotation
            float xOffset = (i - middleIndex) * cardSpacing; // X position based on card index
            float angleOffset = (middleIndex - i) * fanAngle / cardCount; // Rotation based on index (inverted for n-shape)

            // Adjust Y-position for cards on the outside to make them visually lower
            float distanceFromMiddle = Mathf.Abs(i - middleIndex); // Distance from the middle card
            float yOffset = -distanceFromMiddle * maxYOffset; // Lower the card farther from the center

            // Special case for the middle card in odd card counts (slightly lower it)
            if (hasMiddleCard && i == Mathf.RoundToInt(middleIndex))
            {
                yOffset -= middleCardOffset; // Apply a slight downward offset
            }

            // Apply the calculated position and rotation
            Vector3 cardPosition = new Vector3(xOffset, yOffset, 0);
            Quaternion cardRotation = Quaternion.Euler(0, 0, angleOffset); // Apply rotation for fan effect

            // Store the original positions and rotations to revert them after hover
            originalPositions[card] = cardPosition;
            originalRotations[card] = cardRotation;

            // Apply position and rotation
            card.localPosition = cardPosition;
            card.localRotation = cardRotation;

            // Reset scale if it was modified
            card.localScale = Vector3.one;
        }
    }


    // Method to add hover listeners for each card without deleting existing EventTriggers
    private void AddHoverListeners(GameObject card)
    {
        EventTrigger trigger = card.GetComponent<EventTrigger>() ?? card.AddComponent<EventTrigger>();

        // Only add new hover listeners, do not clear existing events
        if (!HasHoverListeners(trigger))
        {
            // Hover over event
            EventTrigger.Entry hoverEntry = new EventTrigger.Entry();
            hoverEntry.eventID = EventTriggerType.PointerEnter;
            hoverEntry.callback.AddListener((data) => { OnCardHoverEnter(card); });
            trigger.triggers.Add(hoverEntry);

            // Hover out event
            EventTrigger.Entry hoverExit = new EventTrigger.Entry();
            hoverExit.eventID = EventTriggerType.PointerExit;
            hoverExit.callback.AddListener((data) => { OnCardHoverExit(card); });
            trigger.triggers.Add(hoverExit);
        }
    }

    // Method to check if hover listeners are already present
    private bool HasHoverListeners(EventTrigger trigger)
    {
        foreach (var entry in trigger.triggers)
        {
            if (entry.eventID == EventTriggerType.PointerEnter || entry.eventID == EventTriggerType.PointerExit)
            {
                return true; // Hover listeners already exist
            }
        }
        return false;
    }

    // Method for handling the hover enter (straighten the card)
    public void OnCardHoverEnter(GameObject card)
    {
        // Straighten the card
        card.transform.localRotation = Quaternion.Euler(0, 0, 0);
        card.transform.localPosition += new Vector3(0, hoverOffset, 0); // Move the card upwards
        card.transform.localScale = new Vector3(hoverScale, hoverScale, hoverScale); // Scale up the card
    }

    // Method for handling the hover exit (return the card to its original position and rotation)
    public void OnCardHoverExit(GameObject card)
    {
        Transform cardTransform = card.transform;

        // Revert to the original position and rotation stored earlier
        if (originalPositions.ContainsKey(cardTransform) && originalRotations.ContainsKey(cardTransform))
        {
            cardTransform.localPosition = originalPositions[cardTransform];
            cardTransform.localRotation = originalRotations[cardTransform];
            cardTransform.localScale = Vector3.one; // Reset scale
        }
    }
}