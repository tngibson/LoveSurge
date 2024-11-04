using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class KeyPressManager : MonoBehaviour
{
    public static KeyPressManager instance;
    [SerializeField] InputAction exit;
    // Start is called before the first frame update
    private void Awake()
    {
        // keeps all stress values throughout the scenes
        DontDestroyOnLoad(this.gameObject);
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);  // Ensures only one instance of StressManager
        }
        else
        {
            instance = this;
        }
        exit.Enable();
    }
    private void Update()
    {
        if (exit != null) 
        {
            if (exit.IsPressed())
            {
                Application.Quit();
            }
        }
    }
}
