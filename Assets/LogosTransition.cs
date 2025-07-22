using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoTransition : MonoBehaviour
{
    [SerializeField] private float delayInSeconds = 3f; // Time to wait before scene transition

    void Start()
    {
        StartCoroutine(LoadNextSceneAfterDelay());
    }

    private IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(delayInSeconds);
        SceneManager.LoadScene(1);
    }
}
