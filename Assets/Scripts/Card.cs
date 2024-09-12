using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] public int cost = 0;
    [SerializeField] public TextMeshProUGUI numText;
    private string type;
    public string Type { get { return type;} set { type = value;} }
    [SerializeField] private int power;
    public int Power { get { return power; } set { power = value; } }
    [SerializeField] public GameObject container;
    [SerializeField] public Image background;
    void Start()
    {
        numText.text = Power.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void upgradeCard()
    {
        this.power += 1;
    }

    public void SetImageAlpha(float alpha)
    {
        // Get the current color of the Image
        Color color = background.color;

        // Set the alpha value (range 0 to 1)
        color.a = Mathf.Clamp01(alpha); // Clamp01 ensures alpha is between 0 and 1

        // Apply the new color with the modified alpha
        background.color = color;
    }
}
