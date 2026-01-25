using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerDeckScriptTutorial : PlayerDeckScript
{
    [Space]
    [SerializeField] private List<ScriptedCards> tutorialCards; // List of tutorial cards to include in the deck
    // Initialize the deck on Awake
    private void Start()
    {
        FillDeck();
        Debug.Log("Tutorial Deck Filled with cards.");
    }

    // Fills the deck with cards of each type and power level
    protected override void FillDeck()
    {
        foreach (ScriptedCards scriptedCard in tutorialCards)
        {
            MakeCard(scriptedCard.card, scriptedCard.power);
        }
    }
}

[System.Serializable]
public class ScriptedCards
{
    public Card card;
    public int power;
}