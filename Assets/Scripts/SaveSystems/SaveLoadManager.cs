using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;

    private static List<ISaveable> saveables = new();

    private Dictionary<string, object> loadedData;

    private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void Register(ISaveable saveable)
    {
        if (!saveables.Contains(saveable))
            saveables.Add(saveable);
    }

    public static void Unregister(ISaveable saveable)
    {
        saveables.Remove(saveable);
    }

    public void Save()
    {
        var data = new Dictionary<string, object>();

        foreach (var saveable in saveables)
        {
            var key = saveable.GetType().ToString();
            data[key] = saveable.CaptureState();
        }

        var json = JsonUtility.ToJson(new SerializationWrapper(data), true);
        File.WriteAllText(SavePath, json);

        Debug.Log($"Saved to {SavePath}");
    }

    public void Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("No save file found.");
            return;
        }

        var json = File.ReadAllText(SavePath);
        var wrapper = JsonUtility.FromJson<SerializationWrapper>(json);
        loadedData = wrapper.ToDictionary();

        StartCoroutine(LoadRoutine());
    }

    private IEnumerator LoadRoutine()
    {
        yield return new WaitForEndOfFrame(); // ensure all objects are alive

        foreach (var saveable in saveables)
        {
            var key = saveable.GetType().ToString();
            if (loadedData.TryGetValue(key, out var state))
            {
                saveable.RestoreState(state);
            }
        }

        PauseMenu.instance.ResumeGame();

        Debug.Log("Load complete.");
    }
}

[System.Serializable]
public class SerializationWrapper
{
    public List<string> keys = new();
    public List<string> values = new(); // JSON strings

    public SerializationWrapper(Dictionary<string, object> data)
    {
        foreach (var kvp in data)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value.ToString());
        }
    }

    public Dictionary<string, object> ToDictionary()
    {
        var dict = new Dictionary<string, object>();
        for (int i = 0; i < keys.Count; i++)
        {
            dict[keys[i]] = values[i];
        }
        return dict;
    }
}