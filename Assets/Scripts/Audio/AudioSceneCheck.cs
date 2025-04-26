using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioSceneCheck : MonoBehaviour
{
    public static AudioSceneCheck instance = null;

    public List<string> sceneNamesToDestroy = new List<string>(); // Define your scene names here

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad (gameObject);
            Debug.Log("nope");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("destroy");
        }
    }

    private void Start()
    {
            if (isMatch())
        {
            Destroy(gameObject);
        }
    }

    bool isMatch()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (sceneNamesToDestroy.Contains(currentSceneName))
        {
            return true;
        }
        return false;
    }
}
