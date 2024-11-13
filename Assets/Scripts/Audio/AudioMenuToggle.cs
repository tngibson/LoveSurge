using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AudioMenuToggle : MonoBehaviour
{
    public static AudioMenuToggle instance { get; private set; }
    // Start is called before the first frame update
    public bool menuOpen { get; private set; }
    void Start()
    {
      menuOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            GetPausePressed();
        }
    }
    public void GetPausePressed()
    {
       Debug.Log("Paused");
        menuOpen = true;
    }
}
