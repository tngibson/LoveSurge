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
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad (gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void isMatch()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"Testing Match: {currentSceneName}");
        if (sceneNamesToDestroy.Contains(currentSceneName))
        {
            Destroy(gameObject);

        }
   
    }

    private void OnDestroy()
    {
        Debug.Log("Did get destroyed");
    }
}
