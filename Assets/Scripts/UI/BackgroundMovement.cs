using UnityEngine;
using UnityEngine.UI;

public class DirectionalMovement : MonoBehaviour
{
    public float scrollSpeedX = 0.1f; // Horizontal scroll speed
    public float scrollSpeedY = 0.1f; // Vertical scroll speed
    private RawImage rawImage;
    private Rect uvRect;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        uvRect = rawImage.uvRect;
    }

    void Update()
    {
        uvRect.x += scrollSpeedX * Time.deltaTime;
        uvRect.y += scrollSpeedY * Time.deltaTime;
        rawImage.uvRect = uvRect;
    }
}

