using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LotteScript : MonoBehaviour
{
    public TextMeshProUGUI checkText;
    public GameObject netContainer;

    public void Check()
    {
        checkText.text = "Lorem ipsum";
    }

    public void Net()
    {
        netContainer.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        checkText.text = "";
    }
}
