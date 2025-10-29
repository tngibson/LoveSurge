using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
[CreateAssetMenu()]

public class Choices : ScriptableObject
{
    public List<string> choiceOptions = new List<string>();  // The text of the choices
    public List<ChoicePath> choicePaths = new List<ChoicePath>();  // Stores dialog and speakers for each choice path
}


[System.Serializable]
public class ChoicePath
{
    public List<string> afterChoiceDialogLines = new List<string>(); // Dialog lines after each choice
    public List<string> afterChoiceSpeakersPerLine = new List<string>(); // Speakers after each choice
    public List<SpriteOptions> afterChoiceSpriteOptions = new List<SpriteOptions>(); // Sprites after each choice

    public string statTag; // The stat to increase (e.g., "Charisma")
    public int statValue;  // The value to increase it by

    // Nested choices for branching inside this choice path
    public List<Choices> nestedChoices = new List<Choices>();
}