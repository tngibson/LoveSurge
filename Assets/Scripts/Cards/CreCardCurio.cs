using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreCardCurio : Card
{
    [SerializeField] private string operation;
    protected override void UpdatePowerDisplay()
    {
        UpdateCurioPowerDisplay(operation);
    }
    
    // Set the type of this card to "Cre"
    protected override void SetType()
    {
        Type = "Cre";
    }
}
