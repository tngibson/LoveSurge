using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHighlight : MonoBehaviour
{
    [SerializeField] private Image screenOverlay;
    [SerializeField] private AnimationCurve fadeInCurve;
    [SerializeField] private AnimationCurve fadeOutCurve;
    [SerializeField] private List<HighlightInstruction> highlightInstructions = new();
    [SerializeField] private List<GameObject> highlightReference = new();
    private Dictionary<string, HighlightInstruction> highlightDict = new();

    private Coroutine highlightCoroutine;
    private void Awake()
    {
        // Initialize the dictionary from the list
        // Set the screen overlay for each highlight instruction
        // Make them all invisible at start
        foreach (var instruction in highlightInstructions)
        {
            instruction.Screen = screenOverlay;
            string safename = instruction.groupName.Replace(" ", "_").ToLower();
            highlightDict[safename] = instruction;
        }

        for(int i = 0; i < highlightReference.Count; i++)
        {
            highlightReference[i].SetActive(false);
        }

        Color c = screenOverlay.color;
        screenOverlay.color = new Color(c.r, c.g, c.b, 0f);
    }

    public void HighlightGroup(string groupName)
    {
        //Sanitize group name
        groupName = groupName.Replace(" ", "_").ToLower();

        if(highlightCoroutine != null)
        {
            StopCoroutine(highlightCoroutine);

            for(int i = 0; i < highlightReference.Count; i++)
            {
                highlightReference[i].SetActive(false);
            }
        }

        if (highlightDict.ContainsKey(groupName))
        {
            highlightCoroutine = StartCoroutine(highlightDict[groupName].HighlightCoroutine(fadeInCurve, fadeOutCurve));
        }
        else
        {
            Debug.LogWarning($"Highlight group '{groupName}' not found!");
        }
    }
}

[System.Serializable]
public class HighlightInstruction
{
    private Image screen;
    public string groupName;
    public List<Image> highlightObjects;

    public Image Screen { get { return screen; } set { screen = value; } }

    public IEnumerator HighlightCoroutine(AnimationCurve fadeinCurve, AnimationCurve fadeoutCurve)
    {
        Debug.Log($"Highlighting group: {groupName}");
        screen.raycastTarget = true;
        Color c = screen.color;

        //Set highlights active for masking
        foreach (var img in highlightObjects)
        {
            img.gameObject.SetActive(true);
        }

        //Handle fadein
        float elapsed = 0f;
        float fadein = fadeinCurve.keys[fadeinCurve.length - 1].time;
        while(elapsed < fadein)
        {

            screen.color = new Color(c.r, c.g, c.b, fadeinCurve.Evaluate(elapsed / fadein));
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        //Handle fadeout
        screen.raycastTarget = false;
        elapsed = 0f;
        float fadeout = fadeoutCurve.keys[fadeoutCurve.length - 1].time;
        while(elapsed < fadeout)
        {
            screen.color = new Color(c.r, c.g, c.b, fadeoutCurve.Evaluate(elapsed / fadeout));

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void ClearHighlights()
    {
        foreach (var img in highlightObjects)
        {
            img.gameObject.SetActive(false);
        }
    }
}
