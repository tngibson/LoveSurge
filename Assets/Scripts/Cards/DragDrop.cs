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
    private bool isOverDropZone = false; // Track if card is over a dropzone
    private bool isOverPlayerArea = false;  // Track if card is over the player areas

    private Vector3 initialPositionA;  // Initial position of this card
    private Vector3 initialPositionB;  // Initial position of the target card
    private Card card;

    // Public method to check if the card is being dragged
    public bool IsDragging() => isDragging;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();  // Find GameManager in the scene
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
        card = this.GetComponent<Card>();
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
        isDragging = true;  // Enable dragging
        startParent = transform.parent;  // Store the original parent
        startPos = transform.position;   // Store the original position

        // Move the card to the canvas root to render above other elements
        transform.SetParent(canvas.transform, true);
        transform.SetAsLastSibling();  // Ensure it renders above all other elements

        initialPositionA = transform.position;  // Store the initial position of this card
    }

    // End dragging the card and determine the appropriate action.
    public void EndDrag()
    {
        isDragging = false;  // Disable dragging

        if (targetCard != null)  // Swap with another card if applicable
        {
            SwapCards(this, targetCard, startParent);
        }
        else if (isOverDropZone && currentDropZone.IsEmpty && gameManager.IsTopicSelected)  // Place in dropzone if applicable
        {
            // Remove from the previous dropzone if it was in one
            if (startParent.GetComponent<DropzoneSlot>() != null)
            {
                DropzoneSlot previousDropzone = startParent.GetComponent<DropzoneSlot>();
                dropzoneManager.RemoveCardFromDropzone(previousDropzone.GetSlotNum());
            }

            // Now place in the new dropzone
            PlaceInDropzone();
        }
        else if (isOverPlayerArea)  // Return the card to the player area
        {
            ReturnToPlayerArea();
        }
        else  // Return to original position if no valid drop
        {
            ReturnToStart();
        }
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

        if (parentA.GetComponent<PlayerArea>() != null && parentB.GetComponent<DropzoneSlot>() != null)
        {
            int index = dropzoneManager.GetCardIndex(cardB.GetComponent<Card>());
            cardB.dropzoneManager.RemoveCardFromDropzone(index);
            cardA.dropzoneManager.AddCardToDropzone(cardA.GetComponent<Card>(), index);
        }
        else if (parentA.GetComponent<DropzoneSlot>() != null && parentB.GetComponent<PlayerArea>() != null)
        {
            int index = dropzoneManager.GetCardIndex(cardA.GetComponent<Card>());
            cardA.dropzoneManager.RemoveCardFromDropzone(index);
            cardB.dropzoneManager.AddCardToDropzone(cardB.GetComponent<Card>(), index);
        }
        else if (parentA.GetComponent<DropzoneSlot>() != null && parentB.GetComponent<DropzoneSlot>() != null)
        {
            // Swap the index of the cards in the Dropzone list PlayedCards
            dropzoneManager.SwapCards(dropzoneManager.GetCardIndex(cardA.GetComponent<Card>()), dropzoneManager.GetCardIndex(cardB.GetComponent<Card>()));
        }

        // Swap parents if one of the cards belongs to a dropzone
        cardA.transform.SetParent(parentB, false);  // Assign cardA to cardB's parent (could be a dropzone)
        cardB.transform.SetParent(parentA, false);  // Assign cardB to cardA's original parent

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

        // Recalculate the score of the Dropzone in case it's necessary
        dropzoneManager.CalculateScore();
    }

    // Place the card into the current dropzone.
    private void PlaceInDropzone()
    {
        // Set the card as a child of the dropzone
        transform.SetParent(currentDropZone.transform, false);

        // Reset the local scale to maintain the original card size
        transform.localScale = Vector3.one;

        dropzoneManager.AddCardToDropzone(card, currentDropZone.GetSlotNum());
    }

    // Return the card to the player area.
    private void ReturnToPlayerArea()
    {
        // If the card was previously in a dropzone, remove it from there
        if (startParent.GetComponent<DropzoneSlot>() != null)
        {
            DropzoneSlot previousDropzone = startParent.GetComponent<DropzoneSlot>();
            dropzoneManager.RemoveCardFromDropzone(previousDropzone.GetSlotNum());
        }

        // Set the card's parent to the player area and position it at the end
        transform.SetParent(playerArea.transform, false);

        // Reset the local scale to maintain the original card size
        transform.localScale = Vector3.one;
    }

    // Return the card to its original position if no valid drop occurred.
    private void ReturnToStart()
    {
        transform.SetParent(startParent, false);  // Reset parent to original
        this.transform.localScale = Vector3.one;
        transform.position = startPos;  // Reset position
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collider is another DragDrop component
        DragDrop otherCard = collision.GetComponent<DragDrop>();
        if (otherCard != null && otherCard != this)  // Ensure it's not the same card
        {
            targetCard = otherCard;  // Set the target card for swapping
            initialPositionB = otherCard.transform.position;  // Store its initial position
        }

        // Check if the collider is a DropzoneSlot
        DropzoneSlot zone = collision.GetComponent<DropzoneSlot>();
        if (zone != null)  // If valid dropzone, set it as the current dropzone
        {
            isOverDropZone = true;
            currentDropZone = zone;
        }

        // Check if the collider is the PlayerArea
        if (collision.GetComponent<PlayerArea>() != null)
        {
            isOverPlayerArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Reset the target card if it leaves the collider
        if (collision.GetComponent<DragDrop>() == targetCard)
        {
            targetCard = null;
        }

        // Reset the dropzone if it leaves the collider
        if (collision.GetComponent<DropzoneSlot>() == currentDropZone)
        {
            isOverDropZone = false;
            currentDropZone = null;
        }

        // Reset the player area flag if the card leaves the player area
        if (collision.GetComponent<PlayerArea>() != null)
        {
            isOverPlayerArea = false;
        }
    }
}
