using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD;

public class DragDrop : MonoBehaviour
{
    [SerializeField] private Dropzone dropZone;  // Reference to the Dropzone component
    [SerializeField] private Card card;  // Reference to the Card component
    [SerializeField] private PlayerArea playerArea;  // Reference to the PlayerArea component
    private bool isDragging = false;  // Flag to check if the object is being dragged
    private Transform startParent;  // To store the original parent of the dragged object
    private Vector2 startPos;  // To store the original position of the dragged object
    private GameObject currentDropZone;  // To keep track of the current drop zone
    private bool isOverDropZone = false;  // Flag to check if the object is over a drop zone
    public FMODUnity.EventReference cardPlace;
    public FMODUnity.EventReference cardHovering;


    void Start()
    {
        // Initialize references to Dropzone and PlayerArea components
        GameObject dropZoneObject = GameObject.Find("Dropzone");
        dropZone = dropZoneObject?.GetComponent<Dropzone>();
        
        GameObject playerAreaObject = GameObject.Find("PlayerArea");
        playerArea = playerAreaObject?.GetComponent<PlayerArea>();

        if (dropZone == null || playerArea == null)
        {
            //Debug.LogError("Dropzone or PlayerArea not found in the scene.");
        }

    }

    void Update()
    {
        if (isDragging)
        {
            // Update the position of the dragged object to follow the mouse
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 0; // Ensure the object is in the correct Z plane
            transform.position = mousePosition;
        }
    }

    public void StartDrag()
    {
        // Start dragging the object
        isDragging = true;
        startParent = transform.parent;
        startPos = transform.position;
    }

    public void EndDrag()
    {
        // End dragging the object
        isDragging = false;

        if (isOverDropZone && dropZone != null && dropZone.IsTopicSelected)
        {
            FMODUnity.RuntimeManager.PlayOneShot(cardPlace);
            print("HELP");
            // Drop the object in the drop zone
            transform.SetParent(currentDropZone.transform, false);

            // Check if the card is already in the drop zone's list to avoid duplication
            if (!dropZone.HasCard(card))  // New method to check if card is already present
            {
                dropZone.AddCard(card);
                transform.position = startPos;
            }

            GetComponent<GridElementSwapper>()?.SetFirstSelectedElement(null);
            playerArea = null;
        }
        else
        {
            // Return the object to its original position and parent
            transform.position = startPos;
            transform.SetParent(startParent, false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Set the drop zone flag and reference when the object enters a collider
        isOverDropZone = true;
        currentDropZone = collision.gameObject;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Reset the drop zone flag and reference when the object exits a collider
        if (collision.gameObject == currentDropZone)
        {
            isOverDropZone = false;
            currentDropZone = null;
        }
    }

    // Public getters for various properties
    public bool IsDragging() => isDragging;

    public PlayerArea GetPlayerArea() => playerArea;

    public Dropzone GetDropzone() => dropZone;

    public void OnMouseOver()
    {
        FMODUnity.RuntimeManager.PlayOneShot(cardHovering);
    }
}
