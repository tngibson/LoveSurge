using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaCardCurio : Card
{
    [SerializeField] private string operation;
    protected override void UpdatePowerDisplay()
    {
        UpdateCurioPowerDisplay(operation);
    }
    // Set the type of this card to "Cha"
    protected override void SetType()
    {
        Type = "Cha";
    }
}
