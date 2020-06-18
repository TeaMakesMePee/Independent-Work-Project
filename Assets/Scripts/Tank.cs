using UnityEngine;
using System.Collections;

public class Tank : Division
{
    public Tank() { }

    public override void UseAbility()
    {
        Debug.LogError("Tank");
    }

    public override void UpdateDivisionStats()
    {

    }
}
