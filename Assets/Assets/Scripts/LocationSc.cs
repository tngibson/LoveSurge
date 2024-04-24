using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationScene : MonoBehaviour
{

    public void ChangeScene(string sceneName)
    {
        GameObject sceneObject = transform.Find(sceneName).gameObject;

        sceneObject.SetActive(true);
    }
}
