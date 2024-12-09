using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChecker : MonoBehaviour
{
    [field: SerializeField] public EnumSoundtrack track { get; private set; }
    Scene scene;
    
    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded; // This tells the script to call the function "OnSceneLoaded" when the scene manager detects the scene has finished loading with the parameters of the scene object and the mode of how the scene was loaded
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name + ", Build Index : " + scene.buildIndex.ToString());
        Debug.Log("Load Mode : " + mode);
        MusicManager.instance.AudioSwitcher(track);
        Debug.Log(track);
    }

    // called third
    void Start()
    {
        Debug.Log("Start");
        scene = SceneManager.GetActiveScene();
    }

    // called when the game or scene closes
    void OnDisable()
    {
        Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded; // This tells the script to stop calling OnSceneLoaded when the scene manager detects a new scene has been loaded
    }
}