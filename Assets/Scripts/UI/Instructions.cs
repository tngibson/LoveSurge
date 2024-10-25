using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instructions : MonoBehaviour
{
    [SerializeField] GameObject instructions;
    public void OnSelect()
    {
        instructions.SetActive(true);
    }
    public void onBackSelect()
    {
        instructions.SetActive(false);
    }
}
