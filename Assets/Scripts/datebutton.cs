using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DateButton : MonoBehaviour
{
    [SerializeField] private string dateScene;
    // Start is called before the first frame update
    public void onPress()
    {
        SceneManager.LoadScene(dateScene);
    }
}
