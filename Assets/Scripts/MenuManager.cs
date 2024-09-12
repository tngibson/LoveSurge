using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject instructions;
    private void Awake()
    {
        instructions.SetActive(false);
    }
    public void onCelciClick()
    {
        SceneManager.LoadScene("CelciTest");
    }
    public void onLotteClick()
    {
        SceneManager.LoadScene("LotteTest");
    }
    public void onInstructionClick()
    {
        instructions.SetActive(true);
    }
    public void onBackClick()
    {
        instructions.SetActive(false);
    }
}
