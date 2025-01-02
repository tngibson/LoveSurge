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
    [SerializeField] private AudioSource cardHover; // Reference to the Card Hover Sound effect
    [Tooltip("Means this card can be played on any other card")]
    public bool isWildPlacer = false; // Means this card can be played on any other card
    [Tooltip("Means any card can be played on this card")]
    public bool isWildReceiver = false; // Means any card can be played on this card
    public bool isBottomCard = false;
    public bool isReserveCard = false;

    private Color defaultTextColor;

    private List<string> ignoredTags;

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
        get => power + GetStatOffsetFromCardType();
        set
        {
            power = value;
            UpdatePowerDisplay();  // Automatically update the UI when power is set
        }
    }

    public bool debuffed = false;

    public bool Debuffed
    {
        get => debuffed;
        set
        {
            debuffed = value;
            UpdatePowerDisplay();
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        SetType();             // Set the type of card in derived classes
        defaultTextColor = numText.color;
        defaultTextColor.a = 1f;
        ignoredTags = new List<string>() { StatOffset.STRESS_FOUR };
        UpdatePowerDisplay();  // Initialize the power display on start
        
    }

    // Abstract method to set the card type, which will be implemented in derived classes
    protected abstract void SetType();

    public int GetStatOffsetFromCardType()
    {
        switch (Type)
        {
            case "Cha":
                return Player.GetSafeOffsets()[0].GetAmount(ignoredTags);
            case "Cle":
                return Player.GetSafeOffsets()[1].GetAmount(ignoredTags);
            case "Cou":
                return Player.GetSafeOffsets()[2].GetAmount(ignoredTags);
            case "Cre":
                return Player.GetSafeOffsets()[3].GetAmount(ignoredTags);
            default:
                return 0;
        }
    }

    // Updates the UI text to display the current power
    private void UpdatePowerDisplay()
    {
        if (numText != null)
        {
            numText.text = Power.ToString();
            numText.color = debuffed ? Color.red : defaultTextColor;
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

    public void SetVisiblity(bool visiblity)
    {
        container.SetActive(visiblity);
    }

    // If you aren't using the Equals operator for anything, you could override that and
    // move this over there to simplify your syntax a little
    public bool CompareCards(Card otherCard)
    {
        return this.Type == otherCard.Type || this.Power == otherCard.Power;
    }

    public virtual bool CanPlaceOnCard(Card otherCard)
    {
        // See the variable definitions for explanations of what wildPlacer and wildReceiver mean
        if (isWildPlacer || (otherCard != null && otherCard.isWildReceiver)) return true;
        else return otherCard == null || CompareCards(otherCard);
    }
}
