using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StatUpgrade : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogText; // Where the narrative outcome will display'
    [SerializeField] private TextMeshProUGUI speakerNameText;  // Speaker name output
    private Player playerManager;      // Reference to the Player manager script
    private string playerName;

    private float typewriterSpeed = 0.025f; // Speed of the typewriter effect

    private bool isTypewriting = false;
    private bool skipRequested = false;

    private Dictionary<string, List<string>> placeholderOptions = new Dictionary<string, List<string>>();
    private Dictionary<string, List<string>> baseTextTemplates = new Dictionary<string, List<string>>();

    private List<string> currentDialogLines; // Holds the lines for the current scene
    private int currentLineIndex = 0; // Tracks the current line being displayed

    [SerializeField] private GameObject mapButton;  // Button to return to map

    void Start()
    {
        if (GameObject.Find("PlayerManager") != null)
        {
            playerManager = GameObject.Find("PlayerManager").GetComponent<Player>();
            playerName = playerManager.GetName();
        }

        speakerNameText.text = "";

        InitializeData();
        StartCoroutine(PlayDialog());
    }

    private void Update()
    {
        // Allow only skip input when dialog is playing
        if (isTypewriting && Input.GetButtonDown("Skip"))
        {
            skipRequested = true;
        }
        else if (!isTypewriting && Input.GetButtonDown("Skip"))
        {
            if (currentLineIndex < currentDialogLines.Count)
            {
                currentLineIndex++; // Increment after displaying the line
            }

            if (currentLineIndex == currentDialogLines.Count)
            {
                currentLineIndex++;

                if (playerManager != null)
                {
                    ApplyStatChanges(currentDialogLines);
                }

                mapButton.SetActive(true);
            }
            else
            {
                StartCoroutine(PlayDialog());
            }
        }
    }

    private void InitializeData()
    {
        // Populate placeholder options
        placeholderOptions["__jobtitles__"] = new List<string>()
    {
        "a Lion Tamer", "a Nurse at a Veterinary Clinic", "a Lobotomizer", "an able seaman",
        "a novice acupuncturist", "a Clerk at a bank", "a Bartender", "an Amateur archaeologist",
        "a Lifeguard", "a Dog Trainer", "a Barista", "a Park Ranger"
    };

        placeholderOptions["__silly_line__"] = new List<string>()
    {
        "“Hyah! Hyah! Back beasts!”", "“THERE’S BLOOD EVERYWHERE. OH THE POOR PENGUINS”",
        "“Is this legal?”", "“CAPTAIN, KRAKEN SPOTTED OFF THE STARBOARD BOW!”",
        "“I’ve never done this before so just tell me if it hurts ok? Unless it's supposed to hurt, then don’t tell me.”",
        "“Robbed on my first day? What a way to go…”", "“One Morning Punch Cocktail coming right up!”",
        "“How are there dinosaurs on a man-made island?”", "“No running. OR ELSE I’LL TALK YOU ALL TO DAVY JONES LOCKER”",
        "“FIDO NO! THAT PATH LEADS TO THE CLIFFS. THE CLIFFS FIDO!”", "“SIR, WE’VE GOT A HORDE OF SOCCER MOMS COMING IN AT 12 O’CLOCK.”",
        "“Dude please don’t punch the bears. They’re gonna eat you if you try to punch them.”"
    };

        placeholderOptions["__money__"] = new List<string>()
    {
        "You earn 100 cash.", "You earn 150 cash.", "You earn 75 cash.", "You earn 200 cash."
    };

        placeholderOptions["__person/animal/group__"] = new List<string>()
    {
        "a murder of crows", "a leopard", "a gang of feral capitalists", "the OmniSpark CEO",
        "a pack of really strong beetles", "a dinosaur", "a really cool dog."
    };

        placeholderOptions["__action__"] = new List<string>()
    {
        "feeding on the corpse of a fawn.", "jogging through the woods.", "lifting weights in the outdoor gym.",
        "meditating peacefully.", "hunting prey...", "swimming through a cool stream.", "enjoying nature."
    };

        placeholderOptions["__creative_action__"] = new List<string>()
    {
        "try sculpting a buff man.", "paint a beautiful landscape.", "make a trophy and give it to yourself.",
        "invent a new, efficient and cost-effective method for city planning.", "build a Gundam. A real one. You stomp around in it for a bit.",
        "write an entire novel about wolves kissing."
    };

        placeholderOptions["__state__"] = new List<string>()
    {
        "so well you win an award for it.", "great! You’re very proud of your work.", "okay, but your mom said it was INCREDIBLE!",
        "fine, it’s not what you wanted but oh well. Next time!", "bad, but you feel inspired to try again soon.",
        "bad, you injure yourself in the process."
    };

        placeholderOptions["__topic__"] = new List<string>()
    {
        "the beaching of dozens of whales on the island as of late", "the flight patterns of birds and how they mimic neural networks",
        "the shaky moral ground this company operates on", "the terminator franchise", "cybersecurity and why this company keeps getting hacked."
    };

        placeholderOptions["__research_action__"] = new List<string>()
    {
        "write an overly wordy research paper about it that is impossible to read.",
        "burnout and spend a few hours eating fast food in a broom closet.",
        "create a massive study that completely upends the conversation on the topic.",
        "solve it. Completely.",
        "go to sleep."
    };

        placeholderOptions["__mallshenanigans__"] = new List<string>()
    {
        "an angsty flock of mall goths.", "a child that says 'woah look at that cool person' and points at you.",
        "a store filled with $800 shoes that all look really nice on you.",
        "the perfect pair of pants that are just too expensive."
    };

        placeholderOptions["__charaction__"] = new List<string>()
    {
        "changes your sense of self-worth forever.", "makes you feel really nice about yourself.",
        "makes you reconsider your entire wardrobe."
    };

        // Scene-specific dialog templates
        baseTextTemplates["Office"] = new List<string>()
    {
        "You went to the OMNITEMP OPPORTUNITIES™ office to find a job.",
        "John Fishman: “Hey there buddy! Ready to get to work!”",
        "Today, you were __jobtitles__. __silly_line__",
        "__money__"
    };

        baseTextTemplates["Park"] = new List<string>()
    {
        "You took a jog through the OMNISPARK WYLDLIFEPARK.",
        "Today, you found __person/animal/group__, __action__",
        "Courage +2."
    };

        baseTextTemplates["ArtBuilding"] = new List<string>()
    {
        "You expressed your feelings in the 4RT15T1C G3N3RAT10N Zone™.",
        "While IN THE ZONE™, you __creative_action__. It goes __state__",
        "Creativity +2."
    };

        baseTextTemplates["Observatory"] = new List<string>()
    {
        "Deciding to focus on the research portion of your internship, you go to work.",
        "You investigate __topic__ at your internship.",
        "After intense research, you __research_action__",
        "Cleverness +2."
    };

        baseTextTemplates["GrandMall"] = new List<string>()
    {
        "You take a stroll through the mall.",
        "In order to up your charm, you just kick around and take in the vibes, until you encounter __mallshenanigans__.",
        "You resolve the situation by __charaction__",
        "Charisma +2."
    };
    }

    private IEnumerator PlayDialog()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (!baseTextTemplates.ContainsKey(currentScene))
        {
            dialogText.text = "There's nothing interesting happening here.";
            yield break;
        }

        if (currentLineIndex == 0)
        {
            // Prepare the dialog lines for the current scene
            currentDialogLines = new List<string>();
            foreach (var line in baseTextTemplates[currentScene])
            {
                currentDialogLines.Add(ReplacePlaceholders(line));
            }
        }

        if (currentLineIndex < currentDialogLines.Count)
        {
            // Update speaker and dialog for the current line
            string currentLine = currentDialogLines[currentLineIndex];
            UpdateSpeakerAndDialog(currentLine);

            // Display the dialog line with typewriting effect
            yield return StartCoroutine(TypewriteDialog(dialogText.text));

            // Wait until the skip button is pressed to proceed
            skipRequested = false;
            yield return new WaitUntil(() => skipRequested);

            currentLineIndex++;
        }
    }

    private void UpdateSpeakerAndDialog(string line)
    {
        // Check for speaker prefix (e.g., "John Fishman:")
        if (line.Contains(":"))
        {
            int colonIndex = line.IndexOf(":");
            if (colonIndex > 0)
            {
                string speaker = line.Substring(0, colonIndex).Trim(); // Extract speaker's name
                speakerNameText.text = speaker; // Update speaker name text
                line = line.Substring(colonIndex + 1).Trim(); // Remove speaker name from line
            }
        }
        else if (line.StartsWith("“") && line.EndsWith("”")) // If line is quoted
        {
            speakerNameText.text = "You";
            line = line.Substring(1, line.Length - 2).Trim(); // Remove quotes
        }
        else
        {
            speakerNameText.text = ""; // Default to empty if no quotes or speaker
        }

        dialogText.text = line; // Update the dialog text
    }

    private string ReplacePlaceholders(string template)
    {
        foreach (var placeholder in placeholderOptions)
        {
            while (template.Contains(placeholder.Key))
            {
                string randomReplacement = placeholder.Value[Random.Range(0, placeholder.Value.Count)];
                template = template.Replace(placeholder.Key, randomReplacement);
            }
        }

        return template;
    }

    private IEnumerator TypewriteDialog(string message)
    {
        isTypewriting = true;
        dialogText.text = ""; // Clear the text box
        string currentMessage = "";

        foreach (char letter in message)
        {
            if (skipRequested)
            {
                // Instantly display the full message if skip is pressed during typing
                dialogText.text = message;
                break;
            }

            currentMessage += letter;
            dialogText.text = currentMessage;

            yield return new WaitForSeconds(typewriterSpeed); // Control typing speed
        }

        dialogText.text = message; // Ensure the full message is displayed
        isTypewriting = false;
    }

    private void ApplyStatChanges(List<string> lines)
    {
        foreach (string line in lines)
        {
            if (line.Contains("cash"))
            {
                // Regex to match numeric values, optionally followed by non-digit characters
                var match = System.Text.RegularExpressions.Regex.Match(line, @"\d+");
                if (match.Success)
                {
                    string cleanNumber = match.Value; // Extracted number
                    int cashIncrease = int.Parse(cleanNumber);
                    playerManager.cash += cashIncrease;
                    Debug.Log($"Cash increased by {cashIncrease}. New total: {playerManager.cash}");
                }
                else
                {
                    Debug.LogWarning($"No numeric value found in line: '{line}'");
                }
            }
            else if (line.Contains("+"))
            {
                string[] parts = line.Split('+');
                if (parts.Length == 2)
                {
                    string statName = parts[0].Trim();
                    if (int.TryParse(parts[1].Trim(' ', '.', ','), out int statValue)) // Remove trailing punctuation
                    {
                        if (System.Enum.TryParse<Player.StatType>(statName, out var statType))
                        {
                            int currentStat = playerManager.GetStat(statType);
                            playerManager.SetStat(statType, currentStat + statValue);
                            Debug.Log($"{statType} increased by {statValue}. New value: {currentStat + statValue}");
                        }
                        else
                        {
                            Debug.LogWarning($"Unknown stat type: '{statName}'");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to parse stat value: '{parts[1].Trim()}' in line: '{line}'");
                    }
                }
                else
                {
                    Debug.LogWarning($"Line format invalid for stat change: '{line}'");
                }
            }
        }
    }
}