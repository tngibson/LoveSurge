using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocationWindowManager : MonoBehaviour
{
    public TextMeshProUGUI infoDisplay;
    public string activeName;
    public LocationScene locationScene;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClose()
    {
        gameObject.SetActive(false);
    }

    public void OnEnter()
    {
        locationScene.ChangeScene(activeName);
    }
}
