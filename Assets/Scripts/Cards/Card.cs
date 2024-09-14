using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class Card : MonoBehaviour
{
    // Serialized fields for card attributes and UI elements
    [SerializeField] public int cost = 0;
    [SerializeField] public TextMeshProUGUI numText;
    [SerializeField] public GameObject container;
    [SerializeField] public Image background;

    // Private backing fields with public properties for type and power
    private string type;
    public string Type
    {
        get => type;
        set => type = value;
    }

    [SerializeField] private int power;
    public int Power
    {
        get => power;
        set
        {
            power = value;
            UpdatePowerDisplay();  // Automatically update the UI when power is set
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        UpdatePowerDisplay();  // Initialize the power display on start
        SetType();             // Set the type of card in derived classes
    }

    // Abstract method to set the card type, which will be implemented in derived classes
    protected abstract void SetType();

    // Updates the UI text to display the current power
    private void UpdatePowerDisplay()
    {
        if (numText != null)
        {
            numText.text = power.ToString();
        }
    }

    // Upgrades the card by increasing its power
    public void UpgradeCard()
    {
        Power += 1;
    }

    public void SetImageAlpha(float alpha)
    {
        if (background != null)
        {
            Color color = background.color;
            color.a = Mathf.Clamp01(alpha);  // Clamps alpha between 0 and 1
            background.color = color;
        }
        else
        {
            Debug.LogWarning("Background image is not assigned.");
        }
    }
}
