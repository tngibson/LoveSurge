using UnityEngine;

public class LocationScene : MonoBehaviour
{
    // Activates a scene by its name, if found under the current object
    public void ChangeScene(string sceneName)
    {
        Transform sceneTransform = transform.Find(sceneName); // Find the child transform by name

        if (sceneTransform != null)
        {
            sceneTransform.gameObject.SetActive(true); // Activate the scene if it exists
        }
        else
        {
            Debug.LogWarning($"Scene '{sceneName}' not found under {gameObject.name}");
        }
    }
}
