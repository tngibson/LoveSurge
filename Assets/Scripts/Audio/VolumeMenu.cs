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
    void Start()
    {
        menu.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (AudioMenuToggle.instance.menuOpen)
        {
            ToggleVolumeMenu();
        }
    }
    private void ToggleVolumeMenu()

    {
        //toggle menu OFF
        if (menu.gameObject.activeInHierarchy)
        {
            menu.gameObject.SetActive(false);
        }
        else
        {
            //toggle menu ON
            menu.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }
}

    