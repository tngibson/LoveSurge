using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class VolumeMenu : MonoBehaviour
{
    public static VolumeMenu instance { get; private set; }
    // Start is called before the first frame update
    [Header("Components")]

    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject firstSelected;
    [SerializeField] AudioMenuToggle audioMenuToggle;
    void Start()
    {
        menu.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (audioMenuToggle.menuOpen)
        {
            ToggleVolumeMenu();
        }
    }
    private void ToggleVolumeMenu()

    {
        //toggle menu OFF
        if (menu.activeInHierarchy)
        {
            menu.SetActive(false);
            Debug.Log("Closed");
        }
        else
        {
            //toggle menu ON
            menu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstSelected);
            Debug.Log("Opened");
        }
    }
}

    