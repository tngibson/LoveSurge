using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalMovement : MonoBehaviour
{
    public float scrollSpeed = 1f; // Adjust this to control the speed of the scrolling
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        // Calculate diagonal movement vector
        Vector3 movement = new Vector3(scrollSpeed, -scrollSpeed, 0) * Time.deltaTime;

        // Move the background
        transform.Translate(movement);

        // Check if the background has moved off the screen
        if (transform.position.x > Screen.width + initialPosition.x || transform.position.y < -Screen.height + initialPosition.y)
        {
            // Move the background back to its initial position to create a looping effect
            transform.position = initialPosition;
        }
    }
}
