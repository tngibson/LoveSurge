using System.Collections.Generic;
using UnityEngine;

public class SetObjectActive : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectsToToggle = new List<GameObject>();

    // Toggles each object's active state
    public void ToggleAllObjects()
    {
        foreach (GameObject obj in objectsToToggle)
        {
            if (obj != null)
                obj.SetActive(!obj.activeSelf);
        }
    }

    // You can also force them all on or off if needed
    public void SetAllObjectsActive(bool state)
    {
        foreach (GameObject obj in objectsToToggle)
        {
            if (obj != null)
                obj.SetActive(state);
        }
    }
}
