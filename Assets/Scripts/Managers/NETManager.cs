using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NETManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Leave()
    {
        gameObject.SetActive(false);
        GlobalInformation.instance.ProgressTimeOfDay();

        dialogueText.text = "Lorem Ipsum! Ja?";
    }

    public void Op1()
    {
        dialogueText.text = "Ja!";
    }

    public void Op2()
    {
        dialogueText.text = "Ja?";

    }
    public void Op3()
    {
        dialogueText.text = "Ja...";
    }
}
