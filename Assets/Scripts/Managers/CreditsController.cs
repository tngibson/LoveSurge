using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreditsController : MonoBehaviour
{
    [Header("Scrolling")]
    public RectTransform creditsContainer;
    public float scrollSpeed = 50f;

    [Header("Fade Elements")]
    public CanvasGroup logoGroup;
    public CanvasGroup buttonGroup;

    public float fadeDuration = 2f;

    private bool creditsFinished = false;
    private bool fadingStarted = false;

    private float screenHeight;

    void Start()
    {
        screenHeight = Screen.height;

        logoGroup.alpha = 0f;
        buttonGroup.alpha = 0f;
        buttonGroup.interactable = false;
    }

    void Update()
    {
        if (!creditsFinished)
        {
            ScrollCredits();
        }
        else if (!fadingStarted)
        {
            fadingStarted = true;
            StartCoroutine(FadeSequence());
        }
    }

    void ScrollCredits()
    {
        creditsContainer.anchoredPosition +=
            Vector2.up * scrollSpeed * Time.deltaTime;

        RectTransform canvasRect =
            creditsContainer.GetComponentInParent<Canvas>()
            .GetComponent<RectTransform>();

        // Top of the canvas in anchored space
        float canvasTop = canvasRect.rect.height / 2f;

        // Bottom of the credits in anchored space
        float creditsBottom =
            creditsContainer.anchoredPosition.y -
            (creditsContainer.rect.height / 2f);

        if (creditsBottom >= canvasTop)
        {
            creditsFinished = true;
        }
    }

    System.Collections.IEnumerator FadeSequence()
    {
        yield return FadeCanvasGroup(logoGroup, 0f, 1f);

        yield return new WaitForSeconds(1f);

        yield return FadeCanvasGroup(buttonGroup, 0f, 1f);
        buttonGroup.interactable = true;
    }

    System.Collections.IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to)
    {
        float timer = 0f;
        group.alpha = from;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, timer / fadeDuration);
            yield return null;
        }

        group.alpha = to;
    }
}