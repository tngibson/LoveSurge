using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeInImage : MonoBehaviour
{
    public Image image; // Assign the Image component in the Inspector
    public float fadeDuration = 2f; // Duration of the fade effect in seconds

    private Vector3 startPosition;
    private Vector3 initialScale;

    void Start()
    {
        if (image == null)
        {
            image = GetComponent<Image>(); // Try to get the Image component if not assigned
        }

        Color startColor = image.color;
        startColor.a = 0f; // Start fully transparent
        image.color = startColor;

        startPosition = image.rectTransform.anchoredPosition;
        initialScale = image.rectTransform.localScale; // Store original scale
        image.rectTransform.localScale = initialScale * 0.5f; // Start at half size

        StartCoroutine(FadeIn());
        StartCoroutine(ScaleUp());
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration + 1)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            Color newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;
            yield return null;
        }
    }

    IEnumerator ScaleUp()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float scaleFactor = Mathf.Pow(elapsedTime / fadeDuration, 4); // Exponential scaling
            image.rectTransform.localScale = Vector3.Lerp(initialScale * 0.5f, initialScale, Mathf.Clamp01(scaleFactor));
            yield return null;
        }
    }
}
