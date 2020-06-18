using UnityEngine;
using System.Collections;

public class Division : MonoBehaviour
{
    public Division() { }

    public virtual void Init(float _jumpForce, float _abilityCooldown) { }

    public virtual void UseAbility() { }

    public virtual void UpdateDivisionStats() { }

    public virtual void Jump(bool inAir) { }
}