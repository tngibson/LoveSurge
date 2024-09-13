using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class HouseManager : MonoBehaviour
{
    public GameObject houseContainer;
    public GameObject bedroomContainer;
    public GameObject lotteContainer;
    

    public void OnFrontDoor()
    {
        houseContainer.SetActive(false);
    }

    public void OnBedroom()
    {
        bedroomContainer.SetActive(true);
    }

    public void Clear()
    {
        //set all containers inactive
    }

    public void Lotte()
    {
        lotteContainer.SetActive(true);
    }

}
