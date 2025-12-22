using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text.RegularExpressions;

public class StatUpgrade : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogText; // Where the narrative outcome will display
    [SerializeField] private TextMeshProUGUI speakerNameText;  // Speaker name output
    private Player playerManager;      // Reference to the Player manager script
    private string playerName;

    private float typewriterSpeed = 0.025f; // Speed of the typewriter effect
    private bool isTypewriting = false;
    private bool skipRequested = false;

    private Dictionary<string, List<string>> placeholderOptions = new();
    private Dictionary<string, List<string>> baseTextTemplates = new();

    [SerializeField] private List<string> currentDialogLines; // Holds the lines for the current scene
    [SerializeField] private int currentLineIndex = 0; // Tracks the current line being displayed

    [SerializeField] private GameObject mapButton;  // Button to return to map
    [SerializeField] private GameObject continueIndicator;

    private OfficeJob? selectedOfficeJob;

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
        if (isTypewriting && Input.GetButtonDown("Skip"))
        {
            skipRequested = true;
        }
        else if (!isTypewriting && Input.GetButtonDown("Skip"))
        {
            currentLineIndex++;

            if (currentLineIndex >= currentDialogLines.Count)
            {
                ApplyStatChanges(currentDialogLines);
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
        officeJobs = new List<OfficeJob>
{
    new OfficeJob
    {
        jobTitle = "Lion Tamer",
        sillyLine = "Hyah! Hyah! Back, beasts!"
    },
    new OfficeJob
    {
        jobTitle = "Nurse at a Veterinary Clinic",
        sillyLine = "THERE’S BLOOD EVERYWHERE. OH THE POOR PENGUINS"
    },
    new OfficeJob
    {
        jobTitle = "Lobotomizer",
        sillyLine = "Is this legal?"
    },
    new OfficeJob
    {
        jobTitle = "Able Seaman",
        sillyLine = "CAPTAIN, KRAKEN SPOTTED OFF THE STARBOARD BOW"
    },
    new OfficeJob
    {
        jobTitle = "Novice Acupuncturist",
        sillyLine = "I’ve never done this before, so tell me if it hurts"
    },
    new OfficeJob
    {
        jobTitle = "Bank Clerk",
        sillyLine = "Robbed on my first day. What a way to go"
    },
    new OfficeJob
    {
        jobTitle = "Bartender",
        sillyLine = "One Morning Punch Cocktail coming right up"
    },
    new OfficeJob
    {
        jobTitle = "Amateur Archaeologist",
        sillyLine = "How are there dinosaurs on a man-made island"
    },
    new OfficeJob
    {
        jobTitle = "Lifeguard",
        sillyLine = "No running or I will send you to Davy Jones’ locker"
    },
    new OfficeJob
    {
        jobTitle = "Dog Trainer",
        sillyLine = "FIDO NO! THAT PATH LEADS TO THE CLIFFS"
    },
    new OfficeJob
    {
        jobTitle = "Barista",
        sillyLine = "WE’VE GOT A HORDE OF SOCCER MOMS COMING IN"
    },
    new OfficeJob
    {
        jobTitle = "Park Ranger",
        sillyLine = "Please do not punch the bears"
    }
};

        placeholderOptions["__money__"] = new()
        {
            "You earn 75 cash",
            "You earn 100 cash",
            "You earn 150 cash",
            "You earn 200 cash"
        };

        placeholderOptions["__creature__"] = new()
        {
            "murder of crows",
            "leopard",
            "gang of feral capitalists",
            "dinosaur",
            "pack of extremely strong beetles",
            "very cool dog"
        };

        placeholderOptions["__creature_action__"] = new()
        {
            "that was feeding on the corpse of a fawn",
            "that was jogging through the woods",
            "that was lifting weights at an outdoor gym",
            "that was meditating peacefully",
            "that was hunting prey",
            "that was swimming through a cool stream"
        };

        placeholderOptions["__creative_action__"] = new()
        {
            "sculpt a muscular statue",
            "paint a detailed landscape",
            "build yourself a trophy",
            "design a revolutionary city-planning system",
            "construct a functional Gundam",
            "write an entire novel about wolves kissing"
        };

        placeholderOptions["__outcome__"] = new()
        {
            "and it turns out great",
            "and you feel very proud of the result",
            "and your mom says it is incredible",
            "but it does not turn out how you hoped",
            "and it inspires you to try again",
            "but you injure yourself in the process"
        };

        placeholderOptions["__mall_event__"] = new()
        {
            "an angsty group of mall goths",
            "a child pointing at you in awe",
            "a store selling outrageously expensive shoes",
            "the perfect pair of pants that you cannot afford"
        };

        placeholderOptions["__resolution__"] = new()
        {
            "handling it with unexpected confidence",
            "somehow making it worse but owning it",
            "turning it into a defining character moment"
        };

        placeholderOptions["__arcade_action__"] = new()
        {
            "play several arcade machines",
            "chat with the prize counter attendant",
            "dominate the high-score leaderboard"
        };

        placeholderOptions["__research_topic__"] = new()
        {
            "the recent beaching of whales on the island",
            "bird flight patterns and their similarity to neural networks",
            "the ethically questionable practices of the company",
            "the Terminator franchise",
            "why the company keeps getting hacked"
        };

        placeholderOptions["__research_action__"] = new()
        {
            "write an excessively long research paper about it",
            "burn out and eat fast food alone in a broom closet",
            "produce a massive study that reshapes the discussion",
            "solve the problem entirely",
            "give up and go to sleep"
        };


        baseTextTemplates["Office"] = new()
        {
            "You visit the OMNITEMP OPPORTUNITIES™ office looking for work.",
            "John Fishman: Hey there buddy! Ready to get to work!",
            "Today, you work as {article} __jobtitle__.",
            "You shout, \"__silly_line__\"",
            "__money__."
        };

        baseTextTemplates["Park"] = new()
        {
            "You jog through the OMNISPARK Wildlife Preserve.",
            "You encounter {article} __creature__ __creature_action__.",
            "Courage +2."
        };

        baseTextTemplates["ArtBuilding"] = new()
        {
            "You spend time in the Artistic Generation Zone™.",
            "You decide to __creative_action__ __outcome__.",
            "Creativity +2."
        };

        baseTextTemplates["GrandMall"] = new()
        {
            "You wander through the mall.",
            "You encounter __mall_event__.",
            "You resolve the situation by __resolution__.",
            "Charisma +2."
        };

        baseTextTemplates["Arcade"] = new()
        {
            "You step into the arcade to unwind.",
            "You __arcade_action__, and after a few hours, you feel refreshed.",
            "You reduced your stress."
        };

        baseTextTemplates["Observatory"] = new()
        {
            "You decide to focus on the research side of your internship.",
            "You investigate __research_topic__.",
            "After several hours of work, you __research_action__.",
            "Cleverness +2."
        };
    }

    private IEnumerator PlayDialog()
    {
        string scene = SceneManager.GetActiveScene().name;

        if (currentLineIndex == 0)
        {
            currentDialogLines = new();

            if (scene == "Office")
            {
                selectedOfficeJob = officeJobs[Random.Range(0, officeJobs.Count)];
            }

            foreach (string line in baseTextTemplates[scene])
            {
                currentDialogLines.Add(ReplacePlaceholders(line));
            }
        }

        if (currentLineIndex < currentDialogLines.Count)
        {
            UpdateSpeakerAndDialog(currentDialogLines[currentLineIndex]);
            yield return StartCoroutine(TypewriteDialog(dialogText.text));
            skipRequested = false;
            yield return new WaitUntil(() => skipRequested);
        }
    }

    private string ReplacePlaceholders(string template)
    {
        if (selectedOfficeJob.HasValue)
        {
            template = template.Replace("__jobtitle__", selectedOfficeJob.Value.jobTitle);
            template = template.Replace("__silly_line__", selectedOfficeJob.Value.sillyLine);
        }

        foreach (var entry in placeholderOptions)
        {
            while (template.Contains(entry.Key))
            {
                string replacement = entry.Value[Random.Range(0, entry.Value.Count)];
                template = template.Replace(entry.Key, replacement);
            }
        }

        if (template.Contains("{article}"))
        {
            var match = System.Text.RegularExpressions.Regex.Match(template, @"\{article\}\s+(\w+)");
            if (match.Success)
            {
                string noun = match.Groups[1].Value;
                string article = StartsWithVowelSound(noun) ? "an" : "a";
                template = template.Replace("{article}", article);
            }
        }

        return template;
    }


    private bool StartsWithVowelSound(string word)
    {
        return Regex.IsMatch(word, "^[aeiouAEIOU]");
    }

    private void UpdateSpeakerAndDialog(string line)
    {
        if (line.Contains(":"))
        {
            int idx = line.IndexOf(":");
            speakerNameText.text = line.Substring(0, idx).Trim();
            dialogText.text = line[(idx + 1)..].Trim();
        }
        else
        {
            speakerNameText.text = "";
            dialogText.text = line;
        }
    }

    private IEnumerator TypewriteDialog(string message)
    {
        isTypewriting = true;
        continueIndicator.SetActive(false);
        dialogText.text = "";

        foreach (char c in message)
        {
            if (skipRequested)
            {
                dialogText.text = message;
                break;
            }

            dialogText.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        dialogText.text = message;
        isTypewriting = false;
        continueIndicator.SetActive(true);
    }

    private void ApplyStatChanges(List<string> lines)
    {
        foreach (string line in lines)
        {
            if (line.Contains("cash"))
            {
                Match m = Regex.Match(line, @"\d+");
                if (m.Success)
                {
                    playerManager.cash += int.Parse(m.Value);
                }
            }
            else if (line.Contains("+"))
            {
                string[] parts = line.Split('+');
                if (System.Enum.TryParse(parts[0].Trim(), out Player.StatType stat))
                {
                    int value = int.Parse(parts[1].Trim('.', ' '));
                    playerManager.SetStat(stat, playerManager.GetStat(stat) + value);
                }
            }
        }
    }

    [System.Serializable]
    private struct OfficeJob
    {
        public string jobTitle;
        public string sillyLine;
    }

    private List<OfficeJob> officeJobs = new();

}