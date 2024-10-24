using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HidePanelButton : MonoBehaviour
{
    public enum Direction { Left, Right, Up, Down }
    public Direction moveDirection;   // Variable for the direction of movement.

    public float moveAmount = 100f;   // Variable for the amount the object is moved.
    public float moveDuration = 0.5f; // Duration for the move animation.

    private Vector3 originalPosition; // Store the original position.
    private bool isHidden = false;    // Boolean to track if the object is hidden.
    [SerializeField] private RectTransform parentRectTransform; // To store the parent RectTransform.
    private bool isMoving = false;    // To prevent multiple movements at once.

    void Start()
    {
        // Cache the original position and parent RectTransform.
        originalPosition = parentRectTransform.anchoredPosition;

        // Attach the button click listener.
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    // Method that is triggered when the button is clicked.
    public void OnButtonClick()
    {
        if (isMoving) return; // If already moving, don't allow another move.

        if (isHidden)
        {
            // Move the parent game object back to its original position.
            StartCoroutine(MoveOverTime(parentRectTransform.anchoredPosition, originalPosition));
            isHidden = false;
        }
        else
        {
            // Calculate the new position in the selected direction.
            Vector3 targetPosition = originalPosition;
            switch (moveDirection)
            {
                case Direction.Left:
                    targetPosition.x -= moveAmount;
                    break;
                case Direction.Right:
                    targetPosition.x += moveAmount;
                    break;
                case Direction.Up:
                    targetPosition.y += moveAmount;
                    break;
                case Direction.Down:
                    targetPosition.y -= moveAmount;
                    break;
            }

            // Start the coroutine to move the object.
            StartCoroutine(MoveOverTime(parentRectTransform.anchoredPosition, targetPosition));
            isHidden = true;
        }
    }

    // Coroutine to move the object smoothly over time.
    private IEnumerator MoveOverTime(Vector3 start, Vector3 end)
    {
        isMoving = true;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            // Smoothly interpolate between the start and end positions over time.
            parentRectTransform.anchoredPosition = Vector3.Lerp(start, end, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final position is exactly the end position.
        parentRectTransform.anchoredPosition = end;
        isMoving = false;
    }
}
