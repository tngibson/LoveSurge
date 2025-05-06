using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Runtime.CompilerServices;
using System.Dynamic;
using FMODUnityResonance;
using UnityEngine.SceneManagement;


public class Debugger : MonoBehaviour
{
    [SerializeField] private string nextScene;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftShift))
        {
            SceneManager.LoadScene(nextScene);
            return;
        }

        if (Input.GetKeyDown(KeyCode.L) && Input.GetKey(KeyCode.LeftShift))
        {
            GameManager.instance?.EndGameLoss();
            return;
        }
         if (Input.GetKeyDown(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        {
            GameManager.instance?.EndGameHalfWin();
            return;
        }
         if (Input.GetKeyDown(KeyCode.E) && Input.GetKey(KeyCode.LeftShift))
        {
            GameManager.instance?.EndGameFullWin();
            return;
        }
    }
}