using UnityEngine;
using System.Collections;

public class Damage : Division
{
    public Damage() { }

    public override void Init(float _jumpForce, float _abilityCooldown)
    {
        base.Init(_jumpForce, _abilityCooldown);
    }

    public override void UpdateDivisionStats()
    {
        if (Input.GetMouseButton(0) && theLoadout.readyFire())
        {
            base.Shoot();
        }
        base.UpdateDivisionStats();
    }

    public override void UseAbility()
    {
        if (abilityCooldown <= 0f)
        {
            abilityCooldown = i_abilityCooldown;
        }
    }

    public override void Jump(bool inAir)
    {
        base.Jump(inAir);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }
}
