using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class GridElementSwapper : MonoBehaviour, IPointerClickHandler
{
    private static Transform firstSelectedElement = null; // Holds the first clicked element for swapping
    public float slideDuration = 0.5f; // Duration of the swap animation
    public float clickedOffset = 5f; // Offset applied to an element when selected

    // This function is triggered when the element is clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        // If the element is being dragged, ignore the click
        if (GetComponent<DragDrop>().IsDragging()) return;

        // If no element has been selected yet, select this one
        if (firstSelectedElement == null)
        {
            SelectElement(transform);
        }
        else
        {
            // If the same element is clicked again, deselect it
            if (firstSelectedElement == transform)
            {
                DeselectElement(firstSelectedElement);
            }
            // If the elements are not in the same area, deselect the first one
            else if (!AreInSameArea(firstSelectedElement, transform))
            {
                DeselectElement(firstSelectedElement);
            }
            // Otherwise, swap the elements
            else
            {
                Vector3 position = transform.position;
                position.y += clickedOffset;
                transform.position = position;
                StartCoroutine(SwapElementsCoroutine(firstSelectedElement, transform));
                DeselectElement(firstSelectedElement);
            }
        }
    }

    // Selects an element and applies the clicked offset
    private void SelectElement(Transform element)
    {
        firstSelectedElement = element;
        Vector3 position = element.position;
        position.y += clickedOffset;
        element.position = position;
    }

    // Deselects an element and removes the clicked offset
    private void DeselectElement(Transform element)
    {
        Vector3 position = element.position;
        position.y -= clickedOffset;
        element.position = position;
        firstSelectedElement = null;
    }

    // Checks if two elements are in the same area (using DragDrop component)
    private bool AreInSameArea(Transform elementA, Transform elementB)
    {
        return elementA.GetComponent<DragDrop>().GetPlayerArea() == elementB.GetComponent<DragDrop>().GetPlayerArea();
    }

    // Coroutine to animate the swap of two elements
    private IEnumerator SwapElementsCoroutine(Transform elementA, Transform elementB)
    {
        Vector3 startPosA = elementA.position;
        Vector3 startPosB = elementB.position;

        float elapsedTime = 0f;
        while (elapsedTime < slideDuration)
        {
            float t = elapsedTime / slideDuration;
            elementA.position = Vector3.Lerp(startPosA, startPosB, t);
            elementB.position = Vector3.Lerp(startPosB, startPosA, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final positions are set
        elementA.position = startPosB;
        elementB.position = startPosA;

        // Swap the sibling indices
        SwapSiblingIndices(elementA, elementB);
    }

    // Swaps the sibling indices of two elements to reorder them in the hierarchy
    private void SwapSiblingIndices(Transform elementA, Transform elementB)
    {
        int indexA = elementA.GetSiblingIndex();
        int indexB = elementB.GetSiblingIndex();
        if (elementA.GetComponent<DragDrop>().GetPlayerArea() == null)
        {
            elementA.GetComponent<DragDrop>().GetDropzone().SwapCards(indexA, indexB);
        }
        elementA.SetSiblingIndex(indexB);
        elementB.SetSiblingIndex(indexA);
    }

    // Setter method used for DragDrop script
    public void SetFirstSelectedElement(Transform newElement)
    {
        firstSelectedElement = newElement;
    }
}