using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;

    string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    public bool IsLoading { get; private set; }

    private SaveFile _pendingLoadFile;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public bool HasSaveFile()
    {
        return File.Exists(SavePath);
    }

    public void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("Save file deleted.");
        }
    }

    // ================= SAVE =================

    public void SaveGame()
    {
        var saveables = FindObjectsOfType<MonoBehaviour>(true);
        var data = new Dictionary<string, string>();

        foreach (var mono in saveables)
        {
            if (mono is ISaveable saveable)
            {
                Debug.Log($"Saving: {mono.name} | ID: {saveable.SaveID}");
                data[saveable.SaveID] = saveable.CaptureState();
            }
        }

        SaveFile file = new SaveFile
        {
            sceneName = SceneManager.GetActiveScene().name,
            entries = new List<SaveEntry>()
        };

        foreach (var kvp in data)
        {
            file.entries.Add(new SaveEntry
            {
                id = kvp.Key,
                json = kvp.Value
            });
        }

        string json = JsonUtility.ToJson(file, true);
        File.WriteAllText(SavePath, json);

        Debug.Log("Game Saved.");
    }

    // ================= LOAD =================

    public void LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("No save file found.");
            return;
        }

        IsLoading = true;

        string json = File.ReadAllText(SavePath);
        SaveFile file = JsonUtility.FromJson<SaveFile>(json);

        Debug.Log("Entries in save file:");
        foreach (var entry in file.entries)
            Debug.Log(entry.id);

        StartCoroutine(RestoreRoutine(file));
    }

    IEnumerator RestoreRoutine(SaveFile file)
    {
        IsLoading = true;

        // Store file temporarily
        _pendingLoadFile = file;

        SceneManager.sceneLoaded += OnSceneLoaded;

        yield return SceneManager.LoadSceneAsync(file.sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        var lookup = new Dictionary<string, string>();
        foreach (var entry in _pendingLoadFile.entries)
            lookup[entry.id] = entry.json;

        foreach (var mono in FindObjectsOfType<MonoBehaviour>(true))
        {
            if (mono is ISaveable saveable &&
                lookup.TryGetValue(saveable.SaveID, out string savedJson))
            {
                Debug.Log("Looking for ID: " + saveable.SaveID);

                Debug.Log($"Restoring: {mono.name} | ID: {saveable.SaveID}");
                saveable.RestoreState(savedJson);
            }
        }

        Debug.Log("Game Loaded.");

        IsLoading = false;
        Time.timeScale = 1f;

        if (PauseMenu.instance != null)
            PauseMenu.instance.ForceResumeAfterLoad();
    }

    // ================= DATA STRUCTURES =================

    [System.Serializable]
    class SaveFile
    {
        public string sceneName;
        public List<SaveEntry> entries;
    }

    [System.Serializable]
    class SaveEntry
    {
        public string id;
        public string json;
    }
}