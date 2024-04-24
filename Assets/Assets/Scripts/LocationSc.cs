using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScene(string sceneName)
    {
        GameObject sceneObject = transform.Find(sceneName).gameObject;

        sceneObject.SetActive(true);
    }
}
