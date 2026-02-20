using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EpilogueManager : MonoBehaviour
{
    public static EpilogueManager Instance { get; private set; }

    private Queue<string> epilogueQueue = new Queue<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void BuildEpilogueQueue()
    {
        epilogueQueue.Clear();

        AddCharacterIfEligible("Noki");
        AddCharacterIfEligible("Celci");
        AddCharacterIfEligible("Lotte");
    }

    private void AddCharacterIfEligible(string characterName)
    {
        var data = LocationManager.Instance.characterDates
            .Find(d => d.name == characterName);

        if (data == null)
            return;

        if (!data.allDatesDone)
            return;

        bool romanticRoute = false;

        switch (characterName)
        {
            case "Noki":
                romanticRoute = Player.instance.nokiRomanticRoute;
                break;
            case "Celci":
                romanticRoute = Player.instance.celciRomanticRoute;
                break;
            case "Lotte":
                romanticRoute = Player.instance.lotteRomanticRoute;
                break;
        }

        string route = romanticRoute ? "Romantic" : "Platonic";
        string sceneName = $"{characterName}Epilogue{route}";

        epilogueQueue.Enqueue(sceneName);
    }

    public void StartEpilogues()
    {
        if (epilogueQueue.Count == 0)
        {
            SceneManager.LoadScene("Credits");
            return;
        }

        LoadNextEpilogue();
    }

    public void LoadNextEpilogue()
    {
        if (epilogueQueue.Count == 0)
        {
            SceneManager.LoadScene("Credits");
            return;
        }

        string nextScene = epilogueQueue.Dequeue();
        SceneManager.LoadScene(nextScene);
    }
}