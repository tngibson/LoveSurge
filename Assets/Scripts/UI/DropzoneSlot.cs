using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DropzoneSlot : MonoBehaviour
{
    private Card currentCard;  // The card in this slot
    [SerializeField] private int slotNum; // What slot this is
    public bool IsEmpty => currentCard == null;

    public void Initialize(Dropzone dropzoneManager) => currentCard = null;
    public void SetCard(Card card) => currentCard = card;
    public Card GetCard() => currentCard;
    public void ClearCard() => currentCard = null;
    public int GetSlotNum() => slotNum;
}
