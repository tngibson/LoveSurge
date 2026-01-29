using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleCardCurio : Card
{
    [SerializeField] private string operation;
    protected override void UpdatePowerDisplay()
    {
        UpdateCurioPowerDisplay(operation);
    }
    
    // Set the type of this card to "Cle"
    protected override void SetType()
    {
        Type = "Cle";
    }
}
