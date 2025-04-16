using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [SerializeField] private string fileName;

    private FileDataHandler dataHandler;
    private GameData gameData;
    public static DataPersistenceManager instance { get; private set; }

    private List<IDataPersistence> dataPersistanceObjects;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }
    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistanceObjects = FindAllDataPersitenceObjects();
        LoadGame();
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }
    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        this.gameData = dataHandler.Load();
        if (this.gameData == null)
        {
            Debug.Log("No save data found, starting a new game");
            NewGame();
        }
        foreach (IDataPersistence dataPerpet in dataPersistanceObjects)
        {
            dataPerpet.LoadData(gameData);
            Debug.Log("Load " + dataPerpet);
        }
        Debug.Log("Loaded name: " + gameData.playerName);
    }

    public void SaveGame()
    {
        foreach (IDataPersistence dataPerpet in dataPersistanceObjects)
        {
            dataPerpet.SaveData(ref gameData);
        }
        Debug.Log("Saved name: " + gameData.playerName);
        dataHandler.Save(gameData);
    }

    private List<IDataPersistence> FindAllDataPersitenceObjects() 
    {
        IEnumerable<IDataPersistence> dataPersistanceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistanceObjects);
    }
}
