using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill Check/SkillCheckPaths")]
public class SkillCheckPaths : ScriptableObject
{
    public string skillName;  // Optional: Name of the skill used for this check (e.g., "Charisma")
    public List<string> successDialogLines = new List<string>();  // Dialog lines if the skill check is successful
    public List<string> successSpeakersPerLine = new List<string>();  // Speakers for each success line
    public List<SpriteOptions> successSpriteOptions = new List<SpriteOptions>();  // Sprites for each success line

    public List<string> failureDialogLines = new List<string>();  // Dialog lines if the skill check fails
    public List<string> failureSpeakersPerLine = new List<string>();  // Speakers for each failure line
    public List<SpriteOptions> failureSpriteOptions = new List<SpriteOptions>();  // Sprites for each failure line
}
