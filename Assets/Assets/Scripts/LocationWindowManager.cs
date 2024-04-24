using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocationWindowManager : MonoBehaviour
{
    public TextMeshProUGUI infoDisplay;
    public string activeName;
    public LocationScene locationScene;




    public void OnClose()
    {
        gameObject.SetActive(false);
    }

    public void OnEnter()
    {
        locationScene.ChangeScene(activeName);
        OnClose();
    }
}
