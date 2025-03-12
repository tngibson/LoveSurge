using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionBar : MonoBehaviour
{
    // notes as of 2/1/25 the current percentages for the bar to fill each section: section 1: .22, section 2: .50, section 3: .78, section 4: 1
    public static ConnectionBar instance;
    private int currentCharacterIndex = 0;
    public GameObject connectionBar;
    public float currentConnectionAmt;
    [SerializeField] private GameObject ConnectionBarParent;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);  // Ensures only one instance of Connection Bar
        }
        else
        {
            instance = this;
        }
    }
    void Start()
    {
        currentConnectionAmt = ConnectionManager.instance.connectionList[currentCharacterIndex];
        UpdateConnectionBar();

    }

    public void updateCurrentConnectionAmt()
    {
        currentConnectionAmt++;
    }


    public void UpdateConnectionBar()
    {
        if (currentConnectionAmt == 1)
        {
            ConnectionBarParent.transform.localScale = new Vector3(.22f, ConnectionBarParent.transform.localScale.y, ConnectionBarParent.transform.localScale.z);
        }
        else if (currentConnectionAmt == 2)
        {
            ConnectionBarParent.transform.localScale = new Vector3(.5f, ConnectionBarParent.transform.localScale.y, ConnectionBarParent.transform.localScale.z);
        }
        else if (currentConnectionAmt == 3)
        {
            ConnectionBarParent.transform.localScale = new Vector3(.78f, ConnectionBarParent.transform.localScale.y, ConnectionBarParent.transform.localScale.z);
        }
        else if (currentConnectionAmt == 4)
        {
            ConnectionBarParent.transform.localScale = new Vector3(1f, ConnectionBarParent.transform.localScale.y, ConnectionBarParent.transform.localScale.z);
        }
    }
}
