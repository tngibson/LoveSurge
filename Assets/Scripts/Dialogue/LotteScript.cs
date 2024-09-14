using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LotteScript : MonoBehaviour
{
    // Serialized fields for UI elements and the net container
    [SerializeField] private TextMeshProUGUI checkText;
    [SerializeField] private GameObject netContainer;

    // Updates the checkText
    public void Check()
    {
        checkText.text = "Lorem ipsum";
    }

    // Activates the netContainer
    public void Net()
    {
        netContainer.SetActive(true);
    }

    // Closes the current object and clears the checkText
    public void Close()
    {
        gameObject.SetActive(false);
        checkText.text = "";
    }
}
