using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ToMap : MonoBehaviour
{
    public void onMap()
    {
        SceneManager.LoadScene(sceneName: "Map");
    }
}
