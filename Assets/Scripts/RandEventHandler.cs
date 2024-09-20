using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RandEventHandler : MonoBehaviour
{
    [SerializeField] int ChoiceCounter;
    [SerializeField] string eventName;
    [SerializeField] GameObject mapButton;
    [SerializeField] GameObject button1;
    [SerializeField] GameObject button2;
    [SerializeField] TextMeshProUGUI button1Text;
    [SerializeField] TextMeshProUGUI button2Text;
    [SerializeField] GameObject nextlineButton;
    [SerializeField] private TextMeshProUGUI textOutput;
    [SerializeField] Choices[] choices;
    int activeChoiceIndex = 0;
    protected StreamReader textReader;
    private FileInfo source1;
    [SerializeField] private AudioSource textSFX;
    protected string text = " ";
    int nextLineStr;
    int lineNum = 0;
    Choices activeChoiseInfo;
    FileInfo choiceDialogue;

    // Typewriter variables
    [SerializeField] private float typewriterSpeed = 0.05f;
    private bool isTypewriting = false;
    private bool skipRequested = false;

    void Start()
    {
        InitializeFileSources();
        StartCoroutine(TypewriteText(textOutput, textReader));  // Start with typewriter effect
        button1.SetActive(false);
        button2.SetActive(false);
        mapButton.SetActive(false);
    }

    private void InitializeFileSources()
    {
        source1 = new FileInfo("Assets/Assets/DialogueResources/Dialogue Files/" + eventName + ".txt");
        textReader = source1.OpenText();
    }

    void Update()
    {
        // Check for skip input
        if (isTypewriting && Input.GetButtonDown("Skip"))
        {
            skipRequested = true;  // Skip request triggered
        }
    }

    public IEnumerator TypewriteText(TextMeshProUGUI convoTextOutput, StreamReader stream)
    {
        if (stream != null)
        {
            lineNum++;
            text = stream.ReadLine();
            textSFX.Play();  // Play sound effect when new text appears
            nextLineStr = stream.Peek();

            if (text.Contains("CHOICE"))
            {
                playChoice();
                yield break;  // Exit the coroutine when it's a choice
            }

            // Clear the text before typewriting begins
            convoTextOutput.text = "";
            isTypewriting = true;
            skipRequested = false;

            // Typewrite each character
            foreach (char letter in text.ToCharArray())
            {
                if (skipRequested)
                {
                    // If skip is requested, show the full text immediately
                    convoTextOutput.text = text;
                    break;
                }
                convoTextOutput.text += letter;
                yield return new WaitForSeconds(typewriterSpeed);  // Control typing speed
            }

            isTypewriting = false;

            if (nextLineStr == -1)
            {
                textReader = null;
                if (activeChoiseInfo != null)
                {
                    if (activeChoiseInfo.afterChoiceFilePath == "")
                    {
                        nextlineButton.SetActive(false);
                        mapButton.SetActive(true);
                        yield break;
                    }
                    FileInfo returnDia = new FileInfo(activeChoiseInfo.afterChoiceFilePath);
                    textReader = returnDia.OpenText();
                    activeChoiseInfo = null;
                    activeChoiceIndex++;
                    StartCoroutine(TypewriteText(convoTextOutput, textReader));
                }
                else
                {
                    nextlineButton.SetActive(false);
                    mapButton.SetActive(true);
                }
            }
        }
    }

    public void playChoice()
    {
        nextlineButton.SetActive(false);
        button1.SetActive(true);
        button2.SetActive(true);
        activeChoiseInfo = choices[activeChoiceIndex];
        button1Text.SetText(activeChoiseInfo.choiceOptions[0]);
        button2Text.SetText(activeChoiseInfo.choiceOptions[1]);
    }

    public void onChoice1()
    {
        nextlineButton.SetActive(true);
        button1.SetActive(false);
        button2.SetActive(false);
        choiceDialogue = activeChoiseInfo.InitFileInfo(0);
        textReader = choiceDialogue.OpenText();
        StartCoroutine(TypewriteText(textOutput, textReader));  // Start typewriting for choice 1
    }

    public void onChoice2()
    {
        nextlineButton.SetActive(true);
        button1.SetActive(false);
        button2.SetActive(false);
        choiceDialogue = activeChoiseInfo.InitFileInfo(1);
        textReader = choiceDialogue.OpenText();
        StartCoroutine(TypewriteText(textOutput, textReader));  // Start typewriting for choice 2
    }

    public void onNextLine()
    {
        StartCoroutine(TypewriteText(textOutput, textReader));  // Start typewriting for the next line
    }

    public void onMap()
    {
        SceneManager.LoadScene(sceneName: "Map");
    }
}

