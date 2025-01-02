using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Add to a gameobject to make it persist between scene changes
public class PreventDestroyOnLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
