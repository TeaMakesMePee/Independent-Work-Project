using UnityEngine;
using System.Collections;

public class Damage : Division
{
    public Damage() { }

    public override void UseAbility()
    {
        Debug.LogError("Dmg");
    }

    public override void UpdateDivisionStats()
    {

    }
}
