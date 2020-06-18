using UnityEngine;
using System.Collections;

public class Damage : Division
{
    public Damage() { }

    public new void UseAbility()
    {
        Debug.LogError("Dmg");
    }

    public new void UpdateDivisionStats()
    {

    }
}
