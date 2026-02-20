using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionBar : MonoBehaviour
{
    public static ConnectionBar instance;

    [Header("Connection Settings")]
    private int currentCharacterIndex = 0; // which character date this belongs to | 0 - Noki, 1 - Celci, 2 - Lotte
    public float currentConnectionAmt;

    [Header("Bar Segments")]
    [SerializeField] private GameObject leftBar;
    [SerializeField] private GameObject middleLeftBar;
    [SerializeField] private GameObject middleRightBar;
    [SerializeField] private GameObject rightBar;

    private void Awake()
    {
        // Singleton setup — only one instance allowed
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        // Initialize connection amount from manager
        currentConnectionAmt = ConnectionManager.instance.connectionList[currentCharacterIndex];
        UpdateConnectionBar();
    }

    public void SetCharacterIndex(int characterIndex)
    {
        currentCharacterIndex = characterIndex;
    }

    public int GetCharacterIndex()
    {
        return currentCharacterIndex;
    }

    public void UpdateCurrentConnectionAmt()
    {
        // Increase connection amount (clamp between 0–4)
        currentConnectionAmt = Mathf.Clamp(currentConnectionAmt + 1, 0, 4);

        // Play sound effect
        AudioManager.instance.PlayOneShot(FMODEvents.instance.ConnectionBarUp);

        // Update visuals
        UpdateConnectionBar();
    }

    public void UpdateConnectionBar()
    {
        // Turn all segments off by default
        leftBar.SetActive(false);
        middleLeftBar.SetActive(false);
        middleRightBar.SetActive(false);
        rightBar.SetActive(false);

        // Turn on segments based on currentConnectionAmt
        if (currentConnectionAmt >= 1)
            leftBar.SetActive(true);
        if (currentConnectionAmt >= 2)
            middleLeftBar.SetActive(true);
        if (currentConnectionAmt >= 3)
            middleRightBar.SetActive(true);
        if (currentConnectionAmt >= 4)
        {
            
            
            rightBar.SetActive(true);
        }
    }
}
