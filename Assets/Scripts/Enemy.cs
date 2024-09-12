using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static Enemy Instance;
    [SerializeField] public int health = 3;
    [SerializeField] public TextMeshProUGUI healthText;
    private void Awake()
    {
        Instance= this;
    }
    public void setHealthText()
    {
        healthText.SetText("Enemy Health : " + health);
    }
}
