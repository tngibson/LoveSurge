using UnityEngine;

public class GridElementSwapper : MonoBehaviour
{
    private static Transform firstSelectedElement = null;

    // Optional utility to reset the first selected element, useful if needed by DragDrop
    public void SetFirstSelectedElement(Transform newElement)
    {
        firstSelectedElement = newElement;
    }
}