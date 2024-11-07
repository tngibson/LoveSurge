using UnityEngine;

public class DirectionalMovement : MonoBehaviour
{
    public float scrollSpeed = 1f; // Adjust this to control the speed of the scrolling
    public float backgroundWidth; // Set this to the width of the background in world units
    public float backgroundHeight; // Set this to the height of the background in world units

    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        // Move the background diagonally based on scroll speed and deltaTime
        Vector3 movement = new Vector3(scrollSpeed, -scrollSpeed, 0) * Time.deltaTime;
        transform.Translate(movement);

        // Check if the background has moved off the screen horizontally
        if (transform.position.x > initialPosition.x + backgroundWidth)
        {
            transform.position = new Vector3(initialPosition.x - backgroundWidth, transform.position.y, transform.position.z);
        }

        // Check if the background has moved off the screen vertically
        if (transform.position.y < initialPosition.y - backgroundHeight)
        {
            transform.position = new Vector3(transform.position.x, initialPosition.y + backgroundHeight, transform.position.z);
        }
    }
}
