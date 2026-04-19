using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SocialsLink : MonoBehaviour
{
    [SerializeField] private Button button;

    public void OnSelect()
    {
        Application.OpenURL("https://linktr.ee/lovesurgegame");
    }
}