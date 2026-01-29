using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CouCardCurio : Card
{
    [SerializeField] private string operation;
    protected override void UpdatePowerDisplay()
    {
        UpdateCurioPowerDisplay(operation);
    }
    // Set the type of this card to "Cou"
    protected override void SetType()
    {
        Type = "Cou";
    }
}
