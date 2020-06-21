using UnityEngine;
using System.Collections;
using Photon.Pun;

public class Damage : Division
{
    private float f_abilityActive;
    public Damage() { }

    public override void Init(float _jumpForce, float _abilityCooldown)
    {
        base.Init(_jumpForce, _abilityCooldown);
    }

    public override void UpdateDivisionStats()
    {
        if (Input.GetMouseButton(0) && theLoadout.readyFire())
        {
            Shoot();
        }
        if (f_abilityActive > 0f)
        {
            f_abilityActive -= Time.deltaTime;
        }
        base.UpdateDivisionStats();
    }

    public override void UseAbility()
    {
        if (abilityCooldown <= 0f)
        {
            abilityCooldown = i_abilityCooldown;
            f_abilityActive = 1.5f;
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

    public override void Shoot()
    {
        if (theLoadout.GetWeapon().FireBullet())
        {
            photonView.RPC("Shoot", RpcTarget.All, theLoadout.GetWeapon().damage, ((f_abilityActive > 0f) ? theLoadout.GetWeapon().firerate * 1.5f : theLoadout.GetWeapon().firerate));
        }
        else
        {
            theLoadout.CheckReload();
        }
    }
}
