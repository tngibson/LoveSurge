using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Unity.VisualScripting;
using UnityEngine;


public class DragDrop : MonoBehaviour
{
    [SerializeField] private Dropzone dropZone;  // Reference to the Dropzone component
    [SerializeField] private Card card;  // Reference to the Card component
    [SerializeField] private PlayerArea playerArea;  // Reference to the PlayerArea component
    [SerializeField] private Canvas canvas;  // Reference to the UI canvas to ensure drag is rendered above all UI elements
    private bool isDragging = false;  // Flag to check if the object is being dragged
    private Transform startParent;  // To store the original parent of the dragged object
    private Vector2 startPos;  // To store the original position of the dragged object
    private GameObject currentDropZone;  // To keep track of the current drop zone
    private bool isOverDropZone = false;  // Flag to check if the object is over a drop zone

    // Audio
    private EventInstance levelMusic;



    void Start()
    {
        // Initialize references to Dropzone and PlayerArea components
        GameObject dropZoneObject = GameObject.Find("Dropzone");
        dropZone = dropZoneObject?.GetComponent<Dropzone>();

        GameObject playerAreaObject = GameObject.Find("PlayerArea");
        playerArea = playerAreaObject?.GetComponent<PlayerArea>();

        GameObject canvasObject = GameObject.Find("Canvas");
        canvas = canvasObject?.GetComponent<Canvas>();

        if (dropZone == null || playerArea == null)
        {
            Debug.LogError("Dropzone or PlayerArea not found in the scene.");
        }
        InitializeAudio();
        AudioManager.instance.PlayOneShot(FMODEvents.instance.datingStart, this.transform.position);

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

        // Unparent from the current parent and move to the canvas root so it's rendered above all other elements
        transform.SetParent(canvas.transform, true);

        // Bring the dragged object to the front
        transform.SetAsLastSibling(); // Ensures the object is rendered above all other UI elements
    }

    public void EndDrag()
    {
        // End dragging the object
        isDragging = false;

        if (isOverDropZone && dropZone != null && dropZone.IsTopicSelected)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.cardPlaced, this.transform.position);
            // Drop the object in the drop zone
            transform.SetParent(currentDropZone.transform, false);

            // Check if the card is already in the drop zone's list to avoid duplication
            if (!dropZone.HasCard(card))  // New method to check if card is already present
            {
                dropZone.AddCard(card);
                transform.position = startPos;
            }

            GetComponent<GridElementSwapper>()?.SetFirstSelectedElement(null);
        }
        else if (!isOverDropZone)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.cardPlaced, this.transform.position);

            transform.SetParent(playerArea.transform, false);

            if (dropZone != null)
            {
                dropZone.RemoveCard(card);
            }

            GetComponent<GridElementSwapper>()?.SetFirstSelectedElement(null);
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
        // Check if the collider's GameObject has a Dropzone component
        Dropzone zone = collision.gameObject.GetComponent<Dropzone>();

        if (zone != null)  // Only proceed if a Dropzone component is found
        {
            // Set the drop zone flag and reference when the object enters a valid dropzone collider
            isOverDropZone = true;
            currentDropZone = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if the collider's GameObject has a Dropzone component
        Dropzone zone = collision.gameObject.GetComponent<Dropzone>();

        // Only reset the drop zone if the object exiting is the current drop zone
        if (zone != null && collision.gameObject == currentDropZone)
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
        AudioManager.instance.PlayOneShot(FMODEvents.instance.cardHovering, this.transform.position);
    }
    private void InitializeAudio()
    {
        PlayBackgroundMusic();
    }
    private void PlayBackgroundMusic()
    {
        levelMusic.getPlaybackState(out PLAYBACK_STATE playbackState);
        if (playbackState == PLAYBACK_STATE.STOPPED)
        {
            levelMusic.start();
        }

    }
}


