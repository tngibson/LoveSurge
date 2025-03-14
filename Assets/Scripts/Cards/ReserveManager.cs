using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ReserveManager : MonoBehaviour
{
    public List<ReserveSlot> reserveSlots;  // The 4 reserve slots
    public int currentPlayableIndex = 0;
    public int currentOpenIndex = -1;
    public ReserveSlot currentOpenSlot;

    [SerializeField] private Image background;

    [SerializeField] private Sprite normalSlotSprite;
    [SerializeField] private Sprite highlightedSlotSprite;

    public int playableCardsLeft = 4;

    void Awake()
    {
        UpdateReserveSlots();
    }

    public void CardPlayed()
    {
        // Move to the next playable card
        reserveSlots[currentPlayableIndex].cardDragDrop = null;
        reserveSlots[currentPlayableIndex].cardImage = null;

        currentPlayableIndex++;
        currentOpenIndex++;
        playableCardsLeft--;

        if (currentOpenIndex >= 0)
        {
            currentOpenSlot = reserveSlots[currentOpenIndex];
        }


        // Below is the functionality that would effectively refresh the reserve slots when they run out. This is currently not how they work, but I'm leaving it here in case that changes.
        /*
        if (currentPlayableIndex >= reserveSlots.Count)
        {
            // All cards have been played, refresh them
            RegenerateReserveCards();

            // Reset the indices
            currentPlayableIndex = 0;
            currentOpenIndex = -1;
            currentOpenSlot = null;
        }
        */

        // If the above functionality is added back in, this if-statement must be reworked
        UpdateReserveSlots();
    }

    public void CardReturned(Card card)
    {
        // Move to the next playable card
        reserveSlots[currentOpenIndex].cardDragDrop = card.GetComponent<DragDrop>();
        reserveSlots[currentOpenIndex].cardImage = card.background;

        currentPlayableIndex--;
        currentOpenIndex--;
        playableCardsLeft++;

        if (currentOpenIndex >= 0)
        {
            currentOpenSlot = reserveSlots[currentOpenIndex];
        }

        UpdateReserveSlots();
    }

    private void UpdateReserveSlots()
    {
        for (int i = currentPlayableIndex; i < reserveSlots.Count; i++)
        {
            bool isPlayable = (i == currentPlayableIndex);
            reserveSlots[i].SetPlayable(isPlayable);
            reserveSlots[i].cardDragDrop.GetComponent<Collider2D>().enabled = isPlayable;
        }

        for (int i = 0; i < reserveSlots.Count; i++)
        {
            if (i == currentPlayableIndex)
            {
                reserveSlots[i].GetComponent<Image>().sprite = highlightedSlotSprite;
                reserveSlots[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                reserveSlots[i].GetComponent<Image>().sprite = normalSlotSprite;
                reserveSlots[i].GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 1f); // Gray out effect
            }
        }
    }

    private void RegenerateReserveCards()
    {
        foreach (ReserveSlot slot in reserveSlots)
        {
            slot.GenerateNewCard(); // Calls the method to generate a new card
        }
    }

    public void ReservesEmpty()
    {
        foreach (ReserveSlot slot in reserveSlots)
        {
            slot.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 1f); // Gray out effect
        }
        background.color = new Color(0.3f, 0.3f, 0.3f, 1f); // Gray out effect
    }

    public Card GetCurrentPlayableCard()
    {
        return reserveSlots[currentPlayableIndex].card;
    }
}
