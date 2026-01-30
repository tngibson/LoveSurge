using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class Socket : MonoBehaviour
{
    [SerializeField] private Transform[] socketPoints;
    [SerializeField] private TextMeshProUGUI[] socketLabel;

    public void AddToSocket(GameObject obj, int index)
    {
        if(index >= socketPoints.Length)
        {
            Debug.LogWarning("No available socket point for index: " + index);
            return;
        }

        obj.transform.position = socketPoints[index].position;
        obj.transform.SetParent(socketPoints[index]);
        obj.transform.localScale = new Vector3(.4f, .4f, 1f);
        obj.SetActive(true);

        if(obj.TryGetComponent(out GameItem item))
        {
            socketLabel[index].text = item.Description;
        }
    }

    public void ClearSocket(int index)
    {
        if(index >= socketPoints.Length)
        {
            Debug.LogWarning("No available socket point for index: " + index);
            return;
        }

        socketLabel[index].text = "";
    }
}