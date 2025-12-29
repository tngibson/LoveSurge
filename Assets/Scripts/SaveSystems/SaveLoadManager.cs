using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;

    string Path => System.IO.Path.Combine(Application.persistentDataPath, "save.json");

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void SaveGame()
    {
        var saveables = FindObjectsOfType<MonoBehaviour>(true);
        var state = new Dictionary<string, object>();

        foreach (var s in saveables)
            if (s is ISaveable saveable)
                state[s.GetType().ToString()] = saveable.CaptureState();

        File.WriteAllText(Path, JsonUtility.ToJson(new SerializationWrapper(state), true));
        Debug.Log("Game saved.");
    }

    public void LoadGame()
    {
        PauseMenu.instance.ResumeGame();
        
        if (!File.Exists(Path))
        {
            Debug.LogWarning("No save file.");
            return;
        }

        var json = File.ReadAllText(Path);
        var wrapper = JsonUtility.FromJson<SerializationWrapper>(json);

        StartCoroutine(RestoreRoutine(wrapper));
    }

    System.Collections.IEnumerator RestoreRoutine(SerializationWrapper wrapper)
    {
        yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        var state = wrapper.ToDictionary();

        foreach (var s in FindObjectsOfType<MonoBehaviour>(true))
            if (s is ISaveable saveable && state.TryGetValue(s.GetType().ToString(), out var data))
                saveable.RestoreState(data);
    }
}

[System.Serializable]
public class SerializationWrapper
{
    public List<string> keys = new();
    public List<string> values = new();

    public SerializationWrapper(Dictionary<string, object> dict)
    {
        foreach (var kvp in dict)
        {
            keys.Add(kvp.Key);
            values.Add(JsonUtility.ToJson(kvp.Value));
        }
    }

    public Dictionary<string, object> ToDictionary()
    {
        var dict = new Dictionary<string, object>();
        for (int i = 0; i < keys.Count; i++)
            dict[keys[i]] = values[i];
        return dict;
    }
}