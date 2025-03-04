using System.Collections.Generic;
using UnityEngine;

public class ReserveManager : MonoBehaviour
{
    public List<ReserveSlot> reserveSlots;  // The 4 reserve slots
    public int currentPlayableIndex = 0;
    public int currentOpenIndex = -1;
    public ReserveSlot currentOpenSlot;

    void Awake()
    {
        UpdateReserveSlots();
    }

    public void CardPlayed()
    {
        // Move to the next playable card
        reserveSlots[currentPlayableIndex].card = null;
        reserveSlots[currentPlayableIndex].cardImage = null;

        currentPlayableIndex++;
        currentOpenIndex++;

        if (currentOpenIndex >= 0)
        {
            currentOpenSlot = reserveSlots[currentOpenIndex];
        }

        if (currentPlayableIndex >= reserveSlots.Count)
        {
            // All cards have been played, refresh them
            RegenerateReserveCards();

            // Reset the indices
            currentPlayableIndex = 0;
            currentOpenIndex = -1;
            currentOpenSlot = null;
        }

        UpdateReserveSlots();
    }

    public void CardReturned(Card card)
    {
        // Move to the next playable card
        reserveSlots[currentOpenIndex].card = card.GetComponent<DragDrop>();
        reserveSlots[currentOpenIndex].cardImage = card.background;

        currentPlayableIndex--;
        currentOpenIndex--;

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
            reserveSlots[i].card.GetComponent<Collider2D>().enabled = isPlayable;
        }
    }

    private void RegenerateReserveCards()
    {
        foreach (ReserveSlot slot in reserveSlots)
        {
            slot.GenerateNewCard(); // Calls the method to generate a new card
        }
    }
}
