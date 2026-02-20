using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class OnClickScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private UnityEvent OnClickEvent;
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickEvent?.Invoke();
    }
}
