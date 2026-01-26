using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestUnityEvent : MonoBehaviour
{
    public UnityEvent<string> testEvent;
    // Start is called before the first frame update
    void Start()
    {
        testEvent.AddListener(EventFunction);
        
        testEvent.Invoke("Hello from Start!");
        testEvent.Invoke("Another message.");
        testEvent.Invoke("Final message.");
    }

    public void EventFunction(string message)
    {
        Debug.Log("Message: " + message);
    }
}
