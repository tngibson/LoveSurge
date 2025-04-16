using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour, IDataPersistence
{
    public static ConnectionManager instance;
    public List<int> connectionList = new(3); // List of connections for each character, each index corresponds to a character (0: Noki, 1: Lotte, 2: Celci)

    void Awake()
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
        public void increaseConnection(int index,int amount)
    {
        connectionList[index] += amount;
        ConnectionBar.instance.updateCurrentConnectionAmt();
        ConnectionBar.instance.UpdateConnectionBar();

    }

    public void LoadData(GameData data)
    {
        this.connectionList = data.connection;
    }

    public void SaveData(ref GameData data)
    {
        data.connection = this.connectionList;
    }
}
