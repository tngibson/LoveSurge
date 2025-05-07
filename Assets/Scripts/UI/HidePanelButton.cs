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
    [SerializeField] private GameObject buttonObject; // To store the parent RectTransform.
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
            StartCoroutine(MoveAndRotateOverTime(parentRectTransform.anchoredPosition, originalPosition, 90f, -90f));
            isHidden = false;
            AudioManager.instance.PlayOneShot(FMODEvents.instance.WindowOpen);
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
            StartCoroutine(MoveAndRotateOverTime(parentRectTransform.anchoredPosition, targetPosition, -90f, 90f));
            AudioManager.instance.PlayOneShot(FMODEvents.instance.WindowClose);
            isHidden = true;
        }
    }

    // Coroutine to move the object smoothly over time.
    private IEnumerator MoveAndRotateOverTime(Vector3 start, Vector3 end, float startRotation, float endRotation)
    {
        isMoving = true;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;

            // Interpolate position
            parentRectTransform.anchoredPosition = Vector3.Lerp(start, end, t);

            // Interpolate rotation on the Z axis
            float currentRotation = Mathf.Lerp(startRotation, endRotation, t);
            buttonObject.transform.localRotation = Quaternion.Euler(0, 0, currentRotation);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final position and rotation are set precisely
        parentRectTransform.anchoredPosition = end;
        buttonObject.transform.localRotation = Quaternion.Euler(0, 0, endRotation);

        isMoving = false;
    }
}
