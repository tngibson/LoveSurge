using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instructions : MonoBehaviour
{
    [SerializeField] GameObject instructions;
    [SerializeField] GameObject[] tabs;
    int pageNum = 1;

    public void OnSelect()
    {
        instructions.SetActive(true);
        Time.timeScale = 0; // Pause the game
    }

    public void onExitSelect()
    {
        instructions.SetActive(false);
        Time.timeScale = 1; // Resume the game
    }

    public void TurnOnTabs(int t)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].SetActive(false);
        }
        tabs[t - 1].SetActive(true);
    }
}
