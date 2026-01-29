using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Socket : MonoBehaviour
{
    [SerializeField] private Transform[] socketPoints;
    private int currentIndex = 0;
    
    public void AddToSocket(GameObject obj)
    {
        if (currentIndex >= socketPoints.Length)
        {
            Debug.LogWarning("No more socket points available!");
            return;
        }

        obj.transform.position = socketPoints[currentIndex].position;
        obj.transform.SetParent(socketPoints[currentIndex]);
        obj.transform.localScale = new Vector3(.4f, .4f, 1f);
        obj.SetActive(true);

        currentIndex++;
    }
}