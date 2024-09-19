using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RandEventHandler : MonoBehaviour
{
    [SerializeField] int ChoiceCounter; // Reference to how many choices the text has
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
    private float timer = 0f;
    [SerializeField] private AudioSource textSFX;
    protected string text = " ";
    int nextLineStr;
    int lineNum = 0;
    Choices activeChoiseInfo;
    FileInfo choiceDialogue;
    // Start is called before the first frame update
    void Start()
    {
        InitializeFileSources();
        ReadText(textOutput, textReader);
        button1.SetActive(false);
        button2.SetActive(false);
        mapButton.SetActive(false);
    }
    private void InitializeFileSources()
    {
        source1 = new FileInfo("Assets/Assets/DialogueResources/Dialogue Files/" + eventName +".txt");

        textReader = source1.OpenText();
    }
    // Update is called once per frame
    void Update()
    {
        

    }
    public void ReadText(TextMeshProUGUI convoTextOutput, StreamReader stream)
    {
        if (stream != null)
        {
            lineNum++;
            text = stream.ReadLine();
            textSFX.Play();
            nextLineStr = stream.Peek();
            if (text.Contains("CHOICE"))
            {
                playChoice();
                return;
            }
            else
            {
                convoTextOutput.text = text ?? ""; // If the text is null, set it to an empty string
            }
            if (nextLineStr == -1)
            {
                textReader = null;
                if (activeChoiseInfo != null) 
                {
                    if (activeChoiseInfo.afterChoiceFilePath == "") 
                    { 
                        nextlineButton.SetActive(false);
                        mapButton.SetActive(true);
                        return;
                    }
                    FileInfo returnDia = new FileInfo(activeChoiseInfo.afterChoiceFilePath);
                    textReader = returnDia.OpenText();
                    activeChoiseInfo = null;
                    activeChoiceIndex++;
                    ReadText(convoTextOutput, textReader);
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
        ReadText(textOutput, textReader); 
    }
    public void onChoice2() 
    {
        nextlineButton.SetActive(true);
        button1.SetActive(false);
        button2.SetActive(false);
        choiceDialogue = activeChoiseInfo.InitFileInfo(1);
        textReader = choiceDialogue.OpenText();
        ReadText(textOutput, textReader);
    }
    public void onNextLine()
    {
        ReadText(textOutput, textReader);
    }
    public void onMap()
    {
        SceneManager.LoadScene(sceneName: "Map");
    }
}
