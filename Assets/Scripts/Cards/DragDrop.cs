using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour
{
    [SerializeField] private PlayerArea playerArea;   // Reference to the PlayerArea component
    [SerializeField] private Canvas canvas;           // Reference to the UI canvas to render above all UI elements
    [SerializeField] private GameManager gameManager; // Reference to GameManager
    [SerializeField] Dropzone dropzoneManager;        // Reference to Dropzone

    private Transform startParent;    // Store the original parent of the dragged card
    private Vector2 startPos;         // Store the original position of the dragged card
    private bool isDragging = false;  // Track if the card is being dragged

    private DragDrop targetCard;      // The card this one is dropped on (for swapping)
    private DropzoneSlot currentDropZone; // The dropzone this card is over
    private DiscardPile currentDiscard; // The discard pile this card is over
    private bool isOverDropZone = false; // Track if card is over a dropzone
    private bool isOverPlayerArea = false;  // Track if card is over the player areas
    private bool isOverDiscardPile = false;  // Track if card is over the discard pile

    private Vector3 initialPositionA;  // Initial position of this card
    private Vector3 initialPositionB;  // Initial position of the target card
    private Card card;
    private CardHandLayout cardHandLayout;

    private int playerAreaCounter = 0; // Counter to track PlayerArea overlap

    // Public method to check if the card is being dragged
    public bool IsDragging() => isDragging;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();  // Find GameManager in the scene
        cardHandLayout = FindAnyObjectByType<CardHandLayout>();
    }

    void Start()
    {
        // Initialize PlayerArea and Canvas
        playerArea = GameObject.Find("PlayerArea")?.GetComponent<PlayerArea>();
        canvas = GameObject.Find("Canvas")?.GetComponent<Canvas>();
        dropzoneManager = GameObject.Find("CardSlotsPanel").GetComponent<Dropzone>();

        if (playerArea == null)
        {
            Debug.LogError("PlayerArea not found in the scene.");
        }
        card = GetComponent<Card>();
    }

    void Update()
    {
        if (isDragging)
        {
            // Make the card follow the mouse cursor
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 0;  // Ensure it stays in the 2D plane
            transform.position = mousePosition;
        }
    }

    // Start dragging the card.
    public void StartDrag()
    {
        isDragging = true; // Enable dragging
        startParent = transform.parent; // Store the original parent
        startPos = transform.position; // Store the original position

        // Re-enable collider if the card is being dragged out of the dropzone
        GetComponent<Collider2D>().enabled = true;

        // Move the card to the canvas root to render above other elements
        transform.SetParent(canvas.transform, true);
        transform.SetAsLastSibling(); // Ensure it renders above all other elements
    }

    // End dragging the card and determine the appropriate action.
    public void EndDrag()
    {
        isDragging = false;  // Disable dragging

        if (targetCard != null)  // Swap with another card if applicable
        {
            SwapCards(this, targetCard, startParent);
        }
        else if (isOverDropZone && gameManager.IsTopicSelected)  // Place in dropzone if applicable
        {
            PlaceInDropzone(); // Use updated PlaceInDropzone to ensure slot state updates correctly
        }
        else if (isOverPlayerArea)  // Return the card to the player area
        {
            ReturnToPlayerArea();
        }
        else if (isOverDiscardPile)
        {
            DiscardCard();
        }
        else  // Return to original position if no valid drop
        {
            ReturnToStart();
        }

        // Ensure dropzone state is updated after every drag
        dropzoneManager.ValidateDropzoneState();
    }

    // Swap the positions of two cards.
    private void SwapCards(DragDrop cardA, DragDrop cardB, Transform startParent)
    {
        // Store world positions before changing parents
        Vector3 worldPosA = cardA.transform.position;
        Vector3 worldPosB = cardB.transform.position;

        // Handle parent assignment during the swap
        Transform parentA = startParent;
        Transform parentB = cardB.transform.parent;

        if (parentB.GetComponent<PlayerArea>() != null)
        {
            // Swap parents if one of the cards belongs to a dropzone
            cardA.transform.SetParent(parentB, false);  // Assign cardA to cardB's parent (could be a dropzone)
            cardB.transform.SetParent(parentB, false);  // Assign cardB to cardA's original parent

            // Convert world positions to local positions in the new parents
            cardA.transform.localPosition = cardA.transform.parent.InverseTransformPoint(worldPosB);
            cardB.transform.localPosition = cardB.transform.parent.InverseTransformPoint(worldPosA);

            // Ensure both cards keep their original size after swapping
            cardA.transform.localScale = Vector3.one;
            cardB.transform.localScale = Vector3.one;

            // Swap sibling indices to maintain correct hierarchy order
            int indexA = cardA.transform.GetSiblingIndex();
            int indexB = cardB.transform.GetSiblingIndex();
            cardA.transform.SetSiblingIndex(indexB);
            cardB.transform.SetSiblingIndex(indexA);

            cardHandLayout.UpdateCardListAndLayout(); // Ensure layout is updated
        }
    }


    // Place the card into the current dropzone and update slot state.
    private void PlaceInDropzone()
    {
        Card cardComponent = GetComponent<Card>();

        // Check if the card can be placed in the dropzone
        if (dropzoneManager.CanPlaceCard(cardComponent))
        {
            dropzoneManager.AddCardToDropzone(cardComponent); // Add card to the dropzone
            playerArea.RemoveCards(cardComponent);
            transform.SetParent(dropzoneManager.GetDropzone().transform, false); // Set parent to DropzoneSlot
            transform.localScale = Vector3.one; // Reset size
            transform.rotation = Quaternion.identity; // Reset rotation
            RemoveHoverListeners();
        }
        else
        {
            Debug.LogError("Card cannot be placed in the dropzone due to attribute mismatch.");
            ReturnToStart(); // Return the card to its original position
        }
    }

    // Return the card to the player area.
    private void ReturnToPlayerArea()
    {
        Card cardComponent = GetComponent<Card>();

        if (dropzoneManager.GetDropzone().GetCards().Contains(cardComponent))
        {
            dropzoneManager.RemoveCardFromDropzone(); // Remove card from dropzone
        }

        // Re-enable the collider when moving back to the player area
        GetComponent<Collider2D>().enabled = true;

        playerArea.AddCards(cardComponent); // Add card back to the player area
        transform.SetParent(playerArea.transform, false); // Set as a child of PlayerArea in hierarchy
        cardHandLayout.UpdateCardListAndLayout();
        transform.localScale = Vector3.one; // Reset card size
    }


    // Return the card to its original position if no valid drop occurred.
    private void ReturnToStart()
    {
        transform.SetParent(startParent, false);  // Reset parent to original
        this.transform.localScale = Vector3.one;
        transform.position = startPos;  // Reset position
    }

    private void DiscardCard()
    {
        playerArea.RemoveCards(GetComponent<Card>()); // Remove card from player area

        transform.SetParent(currentDiscard.transform, false);  // Reset parent to original

        // Disable DragDrop functionality
        var dragDropComponent = GetComponent<DragDrop>();
        if (dragDropComponent != null)
        {
            dragDropComponent.enabled = false;
        }

        // Deactivate the GameObject
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collider is another DragDrop component
        DragDrop otherCard = collision.GetComponent<DragDrop>();
        if (otherCard != null && otherCard != this)  // Ensure it's not the same card
        {
            targetCard = otherCard;  // Set the target card for swapping
            initialPositionB = otherCard.transform.position;  // Store its initial position
            Debug.Log("Enter Card!");
        }

        // Check if the collider is a DropzoneSlot
        DropzoneSlot zone = collision.GetComponent<DropzoneSlot>();
        if (zone != null)  // If valid dropzone, set it as the current dropzone
        {
            isOverDropZone = true;
            currentDropZone = zone;
            Debug.Log("Enter Dropzone!");
        }

        // Check if the collider is the PlayerArea
        if (collision.GetComponent<PlayerArea>() != null)
        {
            playerAreaCounter++; // Increment counter
            isOverPlayerArea = true; // Ensure it's set to true if we're over the PlayerArea
            Debug.Log("Enter Area!");
        }


        DiscardPile discard = collision.GetComponent<DiscardPile>();
        // Check if the collider is the DiscardPile
        if (collision.GetComponent<DiscardPile>() != null)
        {
            isOverDiscardPile = true; // Ensure it's set to true if we're over the DiscardPile
            currentDiscard = discard;
            Debug.Log("Enter Discard!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Reset the target card if it leaves the collider
        if (collision.GetComponent<DragDrop>() == targetCard)
        {
            targetCard = null;
            Debug.Log("Exit Card!");
        }

        // Reset the dropzone if it leaves the collider
        if (collision.GetComponent<DropzoneSlot>() == currentDropZone)
        {
            isOverDropZone = false;
            currentDropZone = null;
            Debug.Log("Exit Dropzone!");
        }

        // Reset the player area flag if the card leaves the player area
        if (collision.GetComponent<PlayerArea>() != null)
        {
            playerAreaCounter--; // Decrement counter
            if (playerAreaCounter <= 0)
            {
                isOverPlayerArea = false; // Only set to false if counter is zero or less
                Debug.Log("Exit Area!");
            }
        }

        // Check if the collider is the DiscardPile
        if (collision.GetComponent<DiscardPile>() != null)
        {
            isOverDiscardPile = false; // Ensure it's set to true if we're over the DiscardPile
            currentDiscard = null;
            Debug.Log("Exit Discard!");
        }
    }

    // Remove hover listeners from the card
    private void RemoveHoverListeners()
    {
        // Assuming you're using EventTrigger for hover effects
        EventTrigger trigger = GetComponent<EventTrigger>();
        if (trigger != null)
        {
            // Remove all hover-related listeners
            trigger.triggers.RemoveAll(entry => entry.eventID == EventTriggerType.PointerEnter || entry.eventID == EventTriggerType.PointerExit);
        }
    }
}
