using UnityEngine;
using System.Collections;

public class Flank : Division
{
    public Flank() { }

    public new void UseAbility()
    {
        Debug.LogError("Flank");
    }

    public new void UpdateDivisionStats()
    {

    }
}
