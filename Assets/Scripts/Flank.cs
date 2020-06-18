using UnityEngine;
using System.Collections;

public class Flank : Division
{
    public Flank() { }

    public override void UseAbility()
    {
        Debug.LogError("Flank");
    }

    public override void UpdateDivisionStats()
    {

    }
}
