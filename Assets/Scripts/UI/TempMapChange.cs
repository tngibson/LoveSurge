using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempMapChange : MonoBehaviour
{
    // Start is called before the first frame update
    public void ToMapScene()
    {
        SceneManager.LoadScene("WakeUp");
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
